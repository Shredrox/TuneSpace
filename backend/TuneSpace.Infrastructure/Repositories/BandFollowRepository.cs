using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

internal class BandFollowRepository(TuneSpaceDbContext context) : IBandFollowRepository
{
    private readonly TuneSpaceDbContext _context = context;

    async Task<BandFollow?> IBandFollowRepository.GetBandFollowAsync(Guid userId, Guid bandId)
    {
        return await _context.BandFollows
            .FirstOrDefaultAsync(bf => bf.UserId == userId && bf.BandId == bandId);
    }

    async Task<List<User>> IBandFollowRepository.GetBandFollowersAsync(Guid bandId)
    {
        return await _context.BandFollows
            .Where(bf => bf.BandId == bandId)
            .Include(bf => bf.User)
            .Select(bf => bf.User)
            .ToListAsync();
    }

    async Task<List<Band>> IBandFollowRepository.GetUserFollowedBandsAsync(Guid userId)
    {
        return await _context.BandFollows
            .Where(bf => bf.UserId == userId)
            .Include(bf => bf.Band)
            .Select(bf => bf.Band)
            .ToListAsync();
    }

    async Task<int> IBandFollowRepository.GetBandFollowerCountAsync(Guid bandId)
    {
        return await _context.BandFollows
            .CountAsync(bf => bf.BandId == bandId);
    }

    async Task<int> IBandFollowRepository.GetUserFollowedBandsCountAsync(Guid userId)
    {
        return await _context.BandFollows
            .CountAsync(bf => bf.UserId == userId);
    }

    async Task<bool> IBandFollowRepository.IsFollowingBandAsync(Guid userId, Guid bandId)
    {
        return await _context.BandFollows
            .AnyAsync(bf => bf.UserId == userId && bf.BandId == bandId);
    }

    async Task<BandFollow> IBandFollowRepository.InsertBandFollowAsync(BandFollow bandFollow)
    {
        _context.BandFollows.Add(bandFollow);
        await _context.SaveChangesAsync();
        return bandFollow;
    }

    async Task<bool> IBandFollowRepository.DeleteBandFollowAsync(Guid userId, Guid bandId)
    {
        var bandFollow = await _context.BandFollows
            .FirstOrDefaultAsync(bf => bf.UserId == userId && bf.BandId == bandId);

        if (bandFollow is null)
        {
            return false;
        }

        _context.BandFollows.Remove(bandFollow);
        await _context.SaveChangesAsync();
        return true;
    }
}
