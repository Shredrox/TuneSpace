using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

internal class MusicEventRepository(TuneSpaceDbContext context) : IMusicEventRepository
{
    private readonly TuneSpaceDbContext _context = context;

    async Task<List<MusicEvent>> IMusicEventRepository.GetMusicEventsByBandId(Guid bandId)
    {
        return await _context.MusicEvents
            .Where(e => e.BandId == bandId)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
    }

    async Task<MusicEvent?> IMusicEventRepository.GetMusicEventById(Guid eventId)
    {
        return await _context.MusicEvents
            .Include(e => e.Band)
            .FirstOrDefaultAsync(e => e.Id == eventId);
    }

    async Task<List<MusicEvent>> IMusicEventRepository.GetUpcomingMusicEvents(Guid bandId)
    {
        var now = DateTime.UtcNow;
        return await _context.MusicEvents
            .Where(e => e.BandId == bandId && e.EventDate > now && !e.IsCancelled)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
    }

    async Task IMusicEventRepository.InsertMusicEvent(MusicEvent musicEvent)
    {
        _context.MusicEvents.Add(musicEvent);
        await _context.SaveChangesAsync();
    }

    async Task IMusicEventRepository.UpdateMusicEvent(MusicEvent musicEvent)
    {
        musicEvent.UpdatedAt = DateTime.UtcNow;
        _context.MusicEvents.Update(musicEvent);
        await _context.SaveChangesAsync();
    }

    async Task IMusicEventRepository.DeleteMusicEvent(Guid eventId)
    {
        var musicEvent = await _context.MusicEvents.FindAsync(eventId) ??
            throw new KeyNotFoundException($"Music Event with ID {eventId} not found.");
        _context.MusicEvents.Remove(musicEvent);
        await _context.SaveChangesAsync();
    }
}
