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

    async Task<User?> IUserService.GetUserById(string id)
    {
        return await _userRepository.GetUserById(id);
    }

    async Task<User?> IUserService.GetUserByName(string name)
    {
        return await _userRepository.GetUserByName(name);
    }

    async Task<User?> IUserService.GetUserFromRefreshToken(string refreshToken)
    {
        var user = await _userRepository.GetUserByRefreshToken(refreshToken);
        return user ?? null;
    }

    async Task<byte[]?> IUserService.GetProfilePicture(string username)
    {
        var user = await _userRepository.GetUserByName(username);

        if (user == null)
        {
            _logger.LogWarning("User not found for profile picture retrieval: {Username}", username);
            throw new NotFoundException($"User not found: {username}");
        }

        return user.ProfilePicture;
    }

    async Task<List<User>> IUserService.SearchByName(string name)
    {
        var users = await _userRepository.SearchByName(name);

        if (users == null || users.Count == 0)
        {
            _logger.LogWarning("No users found for search: {Search}", name);
            return [];
        }

        return users;
    }

    async Task IUserService.UpdateUserRefreshToken(User user)
    {
        var existingUser = await _userRepository.GetUserById(user.Id.ToString());

        if (existingUser == null)
        {
            return;
        }

        existingUser.RefreshToken = user.RefreshToken;
        existingUser.RefreshTokenValidity = user.RefreshTokenValidity;

        await _userRepository.UpdateUser(user);
    }

    async Task IUserService.UpdateProfilePicture(string username, byte[] profilePicture)
    {
        var user = await _userRepository.GetUserByName(username);

        if (user == null)
        {
            _logger.LogWarning("User not found for profile picture update: {Username}", username);
            throw new NotFoundException($"User not found: {username}");
        }

        user.ProfilePicture = profilePicture;
        await _userRepository.UpdateUser(user);

        _logger.LogInformation("Profile picture updated for user: {Username}", username);
    }
}
