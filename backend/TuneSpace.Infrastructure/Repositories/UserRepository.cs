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

    async Task<User?> IUserRepository.GetUserByIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    async Task<User?> IUserRepository.GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    async Task<User?> IUserRepository.GetUserByNameAsync(string name)
    {
        return await _userManager.FindByNameAsync(name);
    }

    async Task<User?> IUserRepository.GetUserByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenValidity > DateTime.Now.ToUniversalTime());
    }

    async Task<List<User>> IUserRepository.SearchByNameAsync(string name)
    {
        return await _userManager.Users
            .Where(u => u.UserName != null && EF.Functions.ILike(u.UserName, $"{name}%"))
            .Take(5)
            .ToListAsync();
    }

    async Task IUserRepository.InsertUserAsync(User user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, user.Role.ToString());
        }
    }

    async Task IUserRepository.UpdateUserAsync(User user)
    {
        await _userManager.UpdateAsync(user);
    }
}
