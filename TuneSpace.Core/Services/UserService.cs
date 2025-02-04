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

    async Task<List<string>> IUserService.SearchByName(string name)
    { 
        var users = await userRepository.SearchByName(name);

        if (users == null || users.Count == 0)
        {
            throw new NotFoundException("No users found");
        }

        return users;
    }
}