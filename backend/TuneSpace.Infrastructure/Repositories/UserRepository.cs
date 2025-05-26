using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

internal class UserRepository(
    UserManager<User> userManager,
    TuneSpaceDbContext context) : IUserRepository
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly TuneSpaceDbContext _context = context;

    async Task<User?> IUserRepository.GetUserById(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    async Task<User?> IUserRepository.GetUserByEmail(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    async Task<User?> IUserRepository.GetUserByName(string name)
    {
        return await _userManager.FindByNameAsync(name);
    }

    async Task<User?> IUserRepository.GetUserByRefreshToken(string refreshToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenValidity > DateTime.Now.ToUniversalTime());
    }

    async Task<List<User>> IUserRepository.SearchByName(string name)
    {
        return await _userManager.Users
            .Where(u => u.UserName != null && EF.Functions.ILike(u.UserName, $"{name}%"))
            .Take(5)
            .ToListAsync();
    }

    async Task IUserRepository.InsertUser(User user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, user.Role.ToString());
        }
    }

    async Task IUserRepository.UpdateUser(User user)
    {
        await _userManager.UpdateAsync(user);
    }
}
