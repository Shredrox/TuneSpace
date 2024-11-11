using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;

namespace TuneSpace.Infrastructure.Repositories;

public class UserRepository(UserManager<User> userManager) : IUserRepository
{
    public async Task<User?> GetUserByEmail(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }

    public async Task<User?> GetUserByName(string name)
    {
        return await userManager.FindByNameAsync(name);
    }

    public async Task<List<string>> SearchByName(string name)
    {
        return await userManager.Users
            .Where(u => u.UserName.StartsWith(name))
            .Select(u => u.UserName)
            .Take(5)
            .ToListAsync();
    }

    public async Task InsertUser(User user, string password)
    {
        await userManager.CreateAsync(user, password);
    }
    
    public async Task UpdateUser(User user)
    {
        await userManager.UpdateAsync(user);
    }
}