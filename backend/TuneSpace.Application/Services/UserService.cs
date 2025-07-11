using Microsoft.Extensions.Logging;
using TuneSpace.Application.Common;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Exceptions;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Application.Services;

internal class UserService(
    IUserRepository userRepository,
    ILogger<UserService> logger) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILogger<UserService> _logger = logger;

    async Task<User?> IUserService.GetUserByIdAsync(string id)
    {
        return await _userRepository.GetUserByIdAsync(id);
    }

    async Task<User?> IUserService.GetUserByNameAsync(string name)
    {
        return await _userRepository.GetUserByNameAsync(name);
    }

    async Task<byte[]?> IUserService.GetProfilePictureAsync(string username)
    {
        var user = await _userRepository.GetUserByNameAsync(username);

        if (user == null)
        {
            _logger.LogWarning("User not found for profile picture retrieval");
            throw new NotFoundException($"User not found: {username}");
        }

        return user.ProfilePicture;
    }

    async Task<List<User>> IUserService.SearchByNameAsync(string name, string? currentUserId)
    {
        var users = await _userRepository.SearchByNameAsync(name);

        if (users == null || users.Count == 0)
        {
            _logger.LogWarning("No users found for search: {Search}", Helpers.SanitizeForLogging(name));
            return [];
        }

        if (!string.IsNullOrEmpty(currentUserId))
        {
            users = [.. users.Where(u => u.Id.ToString() != currentUserId)];
        }

        return users;
    }

    async Task IUserService.UpdateProfilePictureAsync(string username, byte[] profilePicture)
    {
        var user = await _userRepository.GetUserByNameAsync(username);

        if (user == null)
        {
            _logger.LogWarning("User not found for profile picture update");
            throw new NotFoundException($"User not found: {username}");
        }

        user.ProfilePicture = profilePicture;
        await _userRepository.UpdateUserAsync(user);

        _logger.LogInformation("Profile picture updated for user");
    }

    async Task<List<string>> IUserService.GetActiveUserIdsAsync(int daysBack)
    {
        try
        {
            var activeUsers = await _userRepository.GetActiveUsersAsync(daysBack);
            var userIds = activeUsers.Select(u => u.Id.ToString()).ToList();

            _logger.LogInformation("Found {Count} active users in the last {Days} days", userIds.Count, daysBack);
            return userIds;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active users for the last {Days} days", daysBack);
            return [];
        }
    }

    async Task IUserService.UpdateUserActivityAsync(string userId)
    {
        try
        {
            await _userRepository.UpdateUserLastActiveDateAsync(userId, DateTime.UtcNow);
            _logger.LogDebug("Updated last active date for user");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to update last active date for user");
        }
    }
}
