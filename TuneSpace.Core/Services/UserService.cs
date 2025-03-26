using TuneSpace.Core.Entities;
using TuneSpace.Core.Exceptions;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Core.Services;

internal class UserService(IUserRepository userRepository) : IUserService
{
    async Task<User?> IUserService.GetUserByName(string name)
    {
        return await userRepository.GetUserByName(name);
    }

    async Task<User?> IUserService.GetUserFromRefreshToken(string refreshToken)
    {
        var user = await userRepository.GetUserByRefreshToken(refreshToken);
        return user ?? null;
    }

    async Task<List<string>> IUserService.SearchByName(string name)
    { 
        var users = await userRepository.SearchByName(name);

        if (users == null || users.Count == 0)
        {
            throw new NotFoundException("No users found");
        }

        return users;
    }

    async Task IUserService.UpdateUserRefreshToken(User user)
    {
        var existingUser = await userRepository.GetUserById(user.Id);

        if(existingUser == null)
        {
            return;
        }

        existingUser.RefreshToken = user.RefreshToken;
        existingUser.RefreshTokenValidity = user.RefreshTokenValidity;

        await userRepository.UpdateUser(user);
    }
}