using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

internal class FollowRepository(TuneSpaceDbContext context) : IFollowRepository
{
    private readonly TuneSpaceDbContext _context = context;

    async Task<Follow> IFollowRepository.CreateFollowAsync(Follow follow)
    {
        _context.Follows.Add(follow);
        await _context.SaveChangesAsync();
        return follow;
    }

    async Task<Follow?> IFollowRepository.GetFollowAsync(Guid followerId, Guid userId)
    {
        return await _context.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.UserId == userId);
    }

    async Task<List<User>> IFollowRepository.GetFollowersAsync(Guid userId)
    {
        return await _context.Follows
            .Where(f => f.UserId == userId)
            .Include(f => f.Follower)
            .Select(f => f.Follower)
            .ToListAsync();
    }

    async Task<List<User>> IFollowRepository.GetFollowingAsync(Guid followerId)
    {
        return await _context.Follows
            .Where(f => f.FollowerId == followerId)
            .Include(f => f.User)
            .Select(f => f.User)
            .ToListAsync();
    }

    async Task<int> IFollowRepository.GetFollowerCountAsync(Guid userId)
    {
        return await _context.Follows
            .CountAsync(f => f.UserId == userId);
    }

    async Task<int> IFollowRepository.GetFollowingCountAsync(Guid followerId)
    {
        return await _context.Follows
            .CountAsync(f => f.FollowerId == followerId);
    }

    async Task<bool> IFollowRepository.IsFollowingAsync(Guid followerId, Guid userId)
    {
        return await _context.Follows
            .AnyAsync(f => f.FollowerId == followerId && f.UserId == userId);
    }

    async Task<bool> IFollowRepository.DeleteFollowAsync(Guid followerId, Guid userId)
    {
        var follow = await _context.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.UserId == userId);

        if (follow == null)
        {
            return false;
        }

        _context.Follows.Remove(follow);
        await _context.SaveChangesAsync();
        return true;
    }
}
