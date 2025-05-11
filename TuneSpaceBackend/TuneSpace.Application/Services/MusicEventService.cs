using Microsoft.Extensions.Logging;
using TuneSpace.Core.DTOs.Requests.MusicEvent;
using TuneSpace.Core.DTOs.Responses.MusicEvent;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Application.Services;

internal class MusicEventService(
    IMusicEventRepository musicEventRepository,
    IBandRepository bandRepository,
    ILogger<MusicEventService> logger) : IMusicEventService
{
    private readonly IMusicEventRepository _musicEventRepository = musicEventRepository;
    private readonly IBandRepository _bandRepository = bandRepository;
    private readonly ILogger<MusicEventService> _logger = logger;

    async Task<List<MusicEventResponse>> IMusicEventService.GetAllMusicEvents()
    {
        try
        {
            _logger.LogInformation("Getting all music events");
            var events = await _musicEventRepository.GetAllMusicEvents();

            return events.Select(e => new MusicEventResponse(
                e.Id,
                e.BandId,
                e.Band?.Name ?? "Unknown Band",
                e.Title,
                e.Description,
                e.EventDate,
                e.Location,
                e.VenueAddress,
                e.TicketPrice,
                e.TicketUrl,
                e.IsCancelled,
                e.CreatedAt,
                e.UpdatedAt,
                e.City,
                e.Country
            )).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all music events");
            throw;
        }
    }

    async Task<List<MusicEventResponse>> IMusicEventService.GetMusicEventsByBandId(Guid bandId)
    {
        try
        {
            _logger.LogInformation("Getting events for band with ID {BandId}", bandId);
            var events = await _musicEventRepository.GetMusicEventsByBandId(bandId);
            var band = await _bandRepository.GetBandById(bandId);

            if (band == null)
            {
                _logger.LogWarning("Band with ID {BandId} not found when retrieving events", bandId);
                return new List<MusicEventResponse>();
            }

            return events.Select(e => new MusicEventResponse(
                e.Id,
                e.BandId,
                band.Name ?? "Unknown Band",
                e.Title,
                e.Description,
                e.EventDate,
                e.Location,
                e.VenueAddress,
                e.TicketPrice,
                e.TicketUrl,
                e.IsCancelled,
                e.CreatedAt,
                e.UpdatedAt,
                e.City,
                e.Country
            )).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving events for band with ID {BandId}", bandId);
            throw;
        }
    }

    async Task<MusicEventResponse?> IMusicEventService.GetMusicEventById(Guid eventId)
    {
        try
        {
            _logger.LogInformation("Getting event with ID {EventId}", eventId);
            var musicEvent = await _musicEventRepository.GetMusicEventById(eventId);

            if (musicEvent == null)
            {
                _logger.LogWarning("Event with ID {EventId} not found", eventId);
                return null;
            }

            return new MusicEventResponse(
                musicEvent.Id,
                musicEvent.BandId,
                musicEvent.Band?.Name ?? "Unknown Band",
                musicEvent.Title,
                musicEvent.Description,
                musicEvent.EventDate,
                musicEvent.Location,
                musicEvent.VenueAddress,
                musicEvent.TicketPrice,
                musicEvent.TicketUrl,
                musicEvent.IsCancelled,
                musicEvent.CreatedAt,
                musicEvent.UpdatedAt,
                musicEvent.City,
                musicEvent.Country
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving event with ID {EventId}", eventId);
            throw;
        }
    }

    async Task<List<MusicEventResponse>> IMusicEventService.GetUpcomingMusicEvents(Guid bandId)
    {
        try
        {
            _logger.LogInformation("Getting upcoming events for band with ID {BandId}", bandId);
            var events = await _musicEventRepository.GetUpcomingMusicEvents(bandId);
            var band = await _bandRepository.GetBandById(bandId);

            if (band == null)
            {
                _logger.LogWarning("Band with ID {BandId} not found when retrieving upcoming events", bandId);
                return new List<MusicEventResponse>();
            }

            return events.Select(e => new MusicEventResponse(
                e.Id,
                e.BandId,
                band.Name ?? "Unknown Band",
                e.Title,
                e.Description,
                e.EventDate,
                e.Location,
                e.VenueAddress,
                e.TicketPrice,
                e.TicketUrl,
                e.IsCancelled,
                e.CreatedAt,
                e.UpdatedAt,
                e.City,
                e.Country
            )).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving upcoming events for band with ID {BandId}", bandId);
            throw;
        }
    }

    async Task<MusicEvent?> IMusicEventService.CreateMusicEvent(CreateMusicEventRequest request)
    {
        try
        {
            _logger.LogInformation("Creating new event for band with ID {BandId}", request.BandId);

            var band = await _bandRepository.GetBandById(request.BandId);
            if (band == null)
            {
                _logger.LogWarning("Band with ID {BandId} not found when creating event", request.BandId);
                return null;
            }

            var musicEvent = new MusicEvent
            {
                BandId = request.BandId,
                Title = request.Title,
                Description = request.Description,
                EventDate = request.EventDate,
                Location = request.Location,
                VenueAddress = request.VenueAddress,
                TicketPrice = request.TicketPrice,
                TicketUrl = request.TicketUrl,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow,
                City = request.City,
                Country = request.Country
            };

            await _musicEventRepository.InsertMusicEvent(musicEvent);
            _logger.LogInformation("Event {EventTitle} created successfully with ID {EventId} for band {BandId}",
                musicEvent.Title, musicEvent.Id, musicEvent.BandId);

            return musicEvent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event for band with ID {BandId}", request.BandId);
            throw;
        }
    }

    async Task IMusicEventService.UpdateMusicEvent(UpdateMusicEventRequest request)
    {
        try
        {
            _logger.LogInformation("Updating event with ID {EventId}", request.Id);

            var musicEvent = await _musicEventRepository.GetMusicEventById(request.Id);
            if (musicEvent == null)
            {
                _logger.LogWarning("Event with ID {EventId} not found for update", request.Id);
                return;
            }

            if (!string.IsNullOrEmpty(request.Title))
            {
                musicEvent.Title = request.Title;
            }

            if (request.Description != null)
            {
                musicEvent.Description = request.Description;
            }

            if (request.EventDate.HasValue)
            {
                musicEvent.EventDate = request.EventDate.Value;
            }

            if (request.Location != null)
            {
                musicEvent.Location = request.Location;
            }

            if (request.VenueAddress != null)
            {
                musicEvent.VenueAddress = request.VenueAddress;
            }

            if (request.TicketPrice.HasValue)
            {
                musicEvent.TicketPrice = request.TicketPrice;
            }

            if (request.TicketUrl != null)
            {
                musicEvent.TicketUrl = request.TicketUrl;
            }

            if (request.IsCancelled.HasValue)
            {
                musicEvent.IsCancelled = request.IsCancelled.Value;
            }

            await _musicEventRepository.UpdateMusicEvent(musicEvent);
            _logger.LogInformation("Event with ID {EventId} updated successfully", request.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event with ID {EventId}", request.Id);
            throw;
        }
    }

    async Task IMusicEventService.DeleteMusicEvent(Guid eventId)
    {
        try
        {
            _logger.LogInformation("Deleting event with ID {EventId}", eventId);
            await _musicEventRepository.DeleteMusicEvent(eventId);
            _logger.LogInformation("Event with ID {EventId} deleted successfully", eventId);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Event with ID {EventId} not found for deletion", eventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event with ID {EventId}", eventId);
            throw;
        }
    }
}
