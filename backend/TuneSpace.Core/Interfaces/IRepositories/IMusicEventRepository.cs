using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IRepositories;

/// <summary>
/// Provides data access methods for managing music events in the TuneSpace application.
/// </summary>
public interface IMusicEventRepository
{
    /// <summary>
    /// Retrieves all music events.
    /// </summary>
    /// <returns>A list of all music events.</returns>
    Task<List<MusicEvent>> GetAllMusicEventsAsync();

    /// <summary>
    /// Retrieves all events for a specific band.
    /// </summary>
    /// <param name="bandId">The unique identifier of the band.</param>
    /// <returns>A list of music events for the specified band.</returns>
    Task<List<MusicEvent>> GetMusicEventsByBandIdAsync(Guid bandId);

    /// <summary>
    /// Retrieves a specific event by its unique identifier.
    /// </summary>
    /// <param name="eventId">The unique identifier of the event.</param>
    /// <returns>The event if found; otherwise, null.</returns>
    Task<MusicEvent?> GetMusicEventByIdAsync(Guid eventId);

    /// <summary>
    /// Retrieves upcoming events for a specific band.
    /// </summary>
    /// <param name="bandId">The unique identifier of the band.</param>
    /// <returns>A list of upcoming events for the specified band.</returns>
    Task<List<MusicEvent>> GetUpcomingMusicEventsAsync(Guid bandId);

    /// <summary>
    /// Inserts a new music event into the database.
    /// </summary>
    /// <param name="musicEvent">The music event entity to insert.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InsertMusicEventAsync(MusicEvent musicEvent);

    /// <summary>
    /// Updates an existing music event in the database.
    /// </summary>
    /// <param name="musicEvent">The music event entity with updated information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateMusicEventAsync(MusicEvent musicEvent);

    /// <summary>
    /// Deletes a music event from the database.
    /// </summary>
    /// <param name="eventId">The unique identifier of the event to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteMusicEventAsync(Guid eventId);
}
