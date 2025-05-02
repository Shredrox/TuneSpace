using Microsoft.AspNetCore.Mvc;
using TuneSpace.Core.DTOs.Requests.MusicEvent;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MusicEventController(
    IMusicEventService musicEventService,
    ILogger<MusicEventController> logger) : ControllerBase
{
    private readonly IMusicEventService _musicEventService = musicEventService;
    private readonly ILogger<MusicEventController> _logger = logger;

    [HttpGet("band/{bandId}")]
    public async Task<IActionResult> GetBandEvents(string bandId)
    {
        try
        {
            if (!Guid.TryParse(bandId, out var parsedBandId))
            {
                return BadRequest("Invalid Band ID format");
            }

            var events = await _musicEventService.GetMusicEventsByBandId(parsedBandId);
            return Ok(events);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching events for band with ID: {BandId}", bandId);
            return BadRequest();
        }
    }

    [HttpGet("{eventId}")]
    public async Task<IActionResult> GetEventById(string eventId)
    {
        try
        {
            if (!Guid.TryParse(eventId, out var parsedEventId))
            {
                return BadRequest("Invalid Event ID format");
            }

            var musicEvent = await _musicEventService.GetMusicEventById(parsedEventId);

            if (musicEvent == null)
            {
                return NotFound();
            }

            return Ok(musicEvent);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching event with ID: {EventId}", eventId);
            return BadRequest();
        }
    }

    [HttpGet("band/{bandId}/upcoming")]
    public async Task<IActionResult> GetUpcomingBandEvents(string bandId)
    {
        try
        {
            if (!Guid.TryParse(bandId, out var parsedBandId))
            {
                return BadRequest("Invalid Band ID format");
            }

            var events = await _musicEventService.GetUpcomingMusicEvents(parsedBandId);
            return Ok(events);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching upcoming events for band with ID: {BandId}", bandId);
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateMusicEvent(CreateMusicEventRequest request)
    {
        try
        {
            var musicEvent = await _musicEventService.CreateMusicEvent(request);

            if (musicEvent == null)
            {
                return BadRequest("Could not create event.");
            }

            return CreatedAtAction(nameof(GetEventById), new { eventId = musicEvent.Id }, musicEvent.Id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating music event");
            return BadRequest();
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateMusicEvent(UpdateMusicEventRequest request)
    {
        try
        {
            await _musicEventService.UpdateMusicEvent(request);
            return Ok("Event updated successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating event with ID: {EventId}", request.Id);
            return BadRequest();
        }
    }

    [HttpDelete("{eventId}")]
    public async Task<IActionResult> DeleteMusicEvent(string eventId)
    {
        try
        {
            if (!Guid.TryParse(eventId, out var parsedEventId))
            {
                return BadRequest("Invalid Event ID format");
            }

            await _musicEventService.DeleteMusicEvent(parsedEventId);
            return Ok("Event deleted successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting event with ID: {EventId}", eventId);
            return BadRequest();
        }
    }
}
