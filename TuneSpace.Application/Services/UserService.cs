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

    async Task<User?> IUserService.GetUserByName(string name)
    {
        return await _userRepository.GetUserByName(name);
    }

    async Task<User?> IUserService.GetUserFromRefreshToken(string refreshToken)
    {
        var user = await _userRepository.GetUserByRefreshToken(refreshToken);
        return user ?? null;
    }

    async Task<List<string>> IUserService.SearchByName(string name)
    {
        var users = await _userRepository.SearchByName(name);

        if (users == null || users.Count == 0)
        {
            _logger.LogWarning("No users found for search: {Search}", name);
            throw new NotFoundException("No users found");
        }

        return users;
    }

    async Task IUserService.UpdateUserRefreshToken(User user)
    {
        var existingUser = await _userRepository.GetUserById(user.Id);

        if (existingUser == null)
        {
            return;
        }

        existingUser.RefreshToken = user.RefreshToken;
        existingUser.RefreshTokenValidity = user.RefreshTokenValidity;

        await _userRepository.UpdateUser(user);
    }
}
