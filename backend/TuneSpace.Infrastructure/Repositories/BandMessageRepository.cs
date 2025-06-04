using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

internal class BandMessageRepository(TuneSpaceDbContext context) : IBandMessageRepository
{
    private readonly TuneSpaceDbContext _context = context;

    async Task<BandMessage?> IBandMessageRepository.GetByIdAsync(Guid id)
    {
        return await _context.BandMessages
            .Include(bm => bm.Sender)
            .Include(bm => bm.Band)
            .Include(bm => bm.BandChat)
            .FirstOrDefaultAsync(bm => bm.Id == id);
    }

    async Task<IEnumerable<BandMessage>> IBandMessageRepository.GetChatMessagesAsync(Guid bandChatId, int skip, int take)
    {
        if (skip < 0) skip = 0;
        if (take <= 0) take = 50;

        return await _context.BandMessages
            .Include(bm => bm.Sender)
            .Include(bm => bm.Band)
            .Where(bm => bm.BandChatId == bandChatId)
            .OrderBy(bm => bm.Timestamp)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    async Task<int> IBandMessageRepository.GetUnreadCountAsync(Guid userId, Guid bandChatId)
    {
        return await _context.BandMessages
            .Where(bm => bm.BandChatId == bandChatId &&
                        !bm.IsRead &&
                        (bm.IsFromBand || bm.SenderId != userId))
            .CountAsync();
    }

    async Task<BandMessage?> IBandMessageRepository.GetLastMessageForChatAsync(Guid bandChatId)
    {
        return await _context.BandMessages
            .Where(bm => bm.BandChatId == bandChatId)
            .OrderByDescending(bm => bm.Timestamp)
            .FirstOrDefaultAsync();
    }

    async Task<BandMessage> IBandMessageRepository.InsertAsync(BandMessage bandMessage)
    {
        _context.BandMessages.Add(bandMessage);
        await _context.SaveChangesAsync();
        return bandMessage;
    }

    async Task IBandMessageRepository.UpdateAsync(BandMessage bandMessage)
    {
        _context.BandMessages.Update(bandMessage);
        await _context.SaveChangesAsync();
    }

    async Task IBandMessageRepository.DeleteAsync(Guid id)
    {
        var bandMessage = await _context.BandMessages.FindAsync(id);
        if (bandMessage != null)
        {
            _context.BandMessages.Remove(bandMessage);
            await _context.SaveChangesAsync();
        }
    }

    async Task IBandMessageRepository.MarkAsReadAsync(Guid messageId)
    {
        var message = await _context.BandMessages.FindAsync(messageId);
        if (message != null)
        {
            message.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }
}
