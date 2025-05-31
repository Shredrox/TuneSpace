using Microsoft.Extensions.Logging;
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
            _logger.LogWarning("User not found for profile picture retrieval: {Username}", username);
            throw new NotFoundException($"User not found: {username}");
        }

        return user.ProfilePicture;
    }

    async Task<List<User>> IUserService.SearchByNameAsync(string name)
    {
        var users = await _userRepository.SearchByNameAsync(name);

        if (users == null || users.Count == 0)
        {
            _logger.LogWarning("No users found for search: {Search}", name);
            return [];
        }

        return users;
    }

    async Task IUserService.UpdateProfilePictureAsync(string username, byte[] profilePicture)
    {
        var user = await _userRepository.GetUserByNameAsync(username);

        if (user == null)
        {
            _logger.LogWarning("User not found for profile picture update: {Username}", username);
            throw new NotFoundException($"User not found: {username}");
        }

        user.ProfilePicture = profilePicture;
        await _userRepository.UpdateUserAsync(user);

        _logger.LogInformation("Profile picture updated for user: {Username}", username);
    }
}
