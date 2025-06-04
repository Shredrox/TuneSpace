using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

internal class BandChatRepository(TuneSpaceDbContext context) : IBandChatRepository
{
    private readonly TuneSpaceDbContext _context = context;

    async Task<BandChat?> IBandChatRepository.GetByIdAsync(Guid id)
    {
        return await _context.BandChats
            .Include(bc => bc.User)
            .Include(bc => bc.Band)
            .Include(bc => bc.Messages)
            .FirstOrDefaultAsync(bc => bc.Id == id);
    }

    async Task<BandChat?> IBandChatRepository.GetBandChatAsync(Guid userId, Guid bandId)
    {
        return await _context.BandChats
            .Include(bc => bc.User)
            .Include(bc => bc.Band)
            .Include(bc => bc.Messages)
            .FirstOrDefaultAsync(bc => bc.UserId == userId && bc.BandId == bandId);
    }

    async Task<IEnumerable<BandChat>> IBandChatRepository.GetUserBandChatsAsync(Guid userId)
    {
        return await _context.BandChats
            .Include(bc => bc.Band)
            .Where(bc => bc.UserId == userId)
            .OrderByDescending(bc => bc.LastMessageAt)
            .ToListAsync();
    }

    async Task<IEnumerable<BandChat>> IBandChatRepository.GetBandChatsAsync(Guid bandId)
    {
        return await _context.BandChats
            .Include(bc => bc.User)
            .Include(bc => bc.Band)
            .Where(bc => bc.BandId == bandId)
            .OrderByDescending(bc => bc.LastMessageAt)
            .ToListAsync();
    }

    async Task<BandChat> IBandChatRepository.InsertAsync(BandChat bandChat)
    {
        _context.BandChats.Add(bandChat);
        await _context.SaveChangesAsync();
        return bandChat;
    }

    async Task IBandChatRepository.UpdateAsync(BandChat bandChat)
    {
        _context.BandChats.Update(bandChat);
        await _context.SaveChangesAsync();
    }

    async Task IBandChatRepository.DeleteAsync(Guid id)
    {
        var bandChat = await _context.BandChats.FindAsync(id);
        if (bandChat != null)
        {
            _context.BandChats.Remove(bandChat);
            await _context.SaveChangesAsync();
        }
    }
}
