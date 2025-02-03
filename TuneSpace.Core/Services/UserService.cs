using Microsoft.IdentityModel.Tokens;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Exceptions;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Core.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<User?> GetUserByName(string name)
    {
        return await userRepository.GetUserByName(name);
    }

    public async Task<List<string>> SearchByName(string name)
    { 
        var users = await userRepository.SearchByName(name);
        
        // if (users.IsNullOrEmpty())
        // {
        //     throw new NotFoundException("No users found");
        // }
        
        return users;
    }
}