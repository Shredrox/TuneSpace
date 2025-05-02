using TuneSpace.Core.DTOs.Requests.MusicEvent;
using TuneSpace.Core.DTOs.Responses.MusicEvent;
using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Provides business logic services for managing music events in the TuneSpace application.
/// </summary>
public interface IMusicEventService
{
    /// <summary>
    /// Retrieves all events for a specific band.
    /// </summary>
    /// <param name="bandId">The unique identifier of the band.</param>
    /// <returns>A list of music event responses for the specified band.</returns>
    Task<List<MusicEventResponse>> GetMusicEventsByBandId(Guid bandId);

    /// <summary>
    /// Retrieves a specific event by its unique identifier.
    /// </summary>
    /// <param name="eventId">The unique identifier of the event.</param>
    /// <returns>The event response if found; otherwise, null.</returns>
    Task<MusicEventResponse?> GetMusicEventById(Guid eventId);

    /// <summary>
    /// Retrieves upcoming events for a specific band.
    /// </summary>
    /// <param name="bandId">The unique identifier of the band.</param>
    /// <returns>A list of upcoming event responses for the specified band.</returns>
    Task<List<MusicEventResponse>> GetUpcomingMusicEvents(Guid bandId);

    /// <summary>
    /// Creates a new music event.
    /// </summary>
    /// <param name="request">The data required to create a music event.</param>
    /// <returns>The created music event entity if successful; otherwise, null.</returns>
    Task<MusicEvent?> CreateMusicEvent(CreateMusicEventRequest request);

    /// <summary>
    /// Updates an existing music event.
    /// </summary>
    /// <param name="request">The data containing updates to the music event.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateMusicEvent(UpdateMusicEventRequest request);

    /// <summary>
    /// Deletes a music event.
    /// </summary>
    /// <param name="eventId">The unique identifier of the event to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteMusicEvent(Guid eventId);
}
