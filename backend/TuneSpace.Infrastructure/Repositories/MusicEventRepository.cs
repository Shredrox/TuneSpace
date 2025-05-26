using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

internal class MusicEventRepository(TuneSpaceDbContext context) : IMusicEventRepository
{
    private readonly TuneSpaceDbContext _context = context;

    async Task<List<MusicEvent>> IMusicEventRepository.GetAllMusicEventsAsync()
    {
        return await _context.MusicEvents
            .Include(e => e.Band)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
    }

    async Task<List<MusicEvent>> IMusicEventRepository.GetMusicEventsByBandIdAsync(Guid bandId)
    {
        return await _context.MusicEvents
            .Where(e => e.BandId == bandId)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
    }

    async Task<MusicEvent?> IMusicEventRepository.GetMusicEventByIdAsync(Guid eventId)
    {
        return await _context.MusicEvents
            .Include(e => e.Band)
            .FirstOrDefaultAsync(e => e.Id == eventId);
    }

    async Task<List<MusicEvent>> IMusicEventRepository.GetUpcomingMusicEventsAsync(Guid bandId)
    {
        var now = DateTime.UtcNow;
        return await _context.MusicEvents
            .Where(e => e.BandId == bandId && e.EventDate > now && !e.IsCancelled)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
    }

    async Task IMusicEventRepository.InsertMusicEventAsync(MusicEvent musicEvent)
    {
        _context.MusicEvents.Add(musicEvent);
        await _context.SaveChangesAsync();
    }

    async Task IMusicEventRepository.UpdateMusicEventAsync(MusicEvent musicEvent)
    {
        musicEvent.UpdatedAt = DateTime.UtcNow;
        _context.MusicEvents.Update(musicEvent);
        await _context.SaveChangesAsync();
    }

    async Task IMusicEventRepository.DeleteMusicEventAsync(Guid eventId)
    {
        var musicEvent = await _context.MusicEvents.FindAsync(eventId) ??
            throw new KeyNotFoundException($"Music Event with ID {eventId} not found.");
        _context.MusicEvents.Remove(musicEvent);
        await _context.SaveChangesAsync();
    }
}
