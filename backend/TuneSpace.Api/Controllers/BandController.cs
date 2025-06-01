using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuneSpace.Api.DTOs;
using TuneSpace.Application.Common;
using TuneSpace.Core.DTOs.Requests.Band;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BandController(
    IBandService bandService,
    ILogger<BandController> logger) : ControllerBase
{
    private readonly IBandService _bandService = bandService;
    private readonly ILogger<BandController> _logger = logger;

    [HttpGet("{bandId}")]
    [Authorize]
    public async Task<IActionResult> GetBandById(string bandId)
    {
        if (string.IsNullOrEmpty(bandId))
        {
            return BadRequest("Band ID cannot be null or empty");
        }

        if (!Guid.TryParse(bandId, out var parsedBandId))
        {
            return BadRequest("Invalid Band ID format");
        }

        try
        {
            var band = await _bandService.GetBandByIdAsync(parsedBandId);
            return Ok(band);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching band with ID: {BandId}", bandId);
            return BadRequest();
        }
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetBandByUserId(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("User ID cannot be null or empty");
        }

        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            return BadRequest("Invalid User ID format");
        }

        try
        {
            var band = await _bandService.GetBandByUserIdAsync(userId);
            return Ok(band);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching band for user with ID: {UserId}", userId);
            return BadRequest();
        }
    }

    [HttpGet("{bandId}/image")]
    [Authorize]
    public async Task<IActionResult> GetBandImage(string bandId)
    {
        if (string.IsNullOrEmpty(bandId))
        {
            return BadRequest("Band ID cannot be null or empty");
        }

        if (!Guid.TryParse(bandId, out var parsedBandId))
        {
            return BadRequest("Invalid Band ID format");
        }

        try
        {
            var imageData = await _bandService.GetBandImageAsync(parsedBandId);

            if (imageData is null)
            {
                return NotFound();
            }

            return File(imageData, "image/jpeg");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching band image for ID: {BandId}", bandId);
            return BadRequest();
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateBand([FromForm] BandCreateRequest request)
    {
        try
        {
            var fileDto = await Helpers.ConvertToFileDto(request.Picture);

            var createBandRequest = new CreateBandRequest(
                request.UserId,
                request.Name,
                request.Description,
                request.Genre,
                request.Location,
                fileDto ?? null
            );

            var band = await _bandService.CreateBandAsync(createBandRequest);

            if (band is null)
            {
                return BadRequest();
            }

            return Ok(band.Id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating band");
            return BadRequest();
        }
    }

    [HttpPost("add-member")]
    public async Task<IActionResult> AddMemberToBand([FromBody] AddMemberRequest request)
    {
        var userId = request.UserId;
        var bandId = request.BandId;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(bandId))
        {
            return BadRequest("User ID and Band ID cannot be null or empty");
        }

        if (!Guid.TryParse(userId, out var parsedUserId) || !Guid.TryParse(bandId, out var parsedBandId))
        {
            return BadRequest("Invalid User ID or Band ID format");
        }

        try
        {
            await _bandService.AddMemberToBandAsync(parsedUserId, parsedBandId);
            return Ok("Member added successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error adding member to band with ID: {BandId}", bandId);
            return BadRequest();
        }
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> UpdateBand([FromForm] BandUpdateRequest request)
    {
        try
        {
            var fileDto = await Helpers.ConvertToFileDto(request.Picture);

            var updateBandRequest = new UpdateBandRequest(
                request.Id,
                request.Name,
                request.Description,
                request.Genre,
                request.SpotifyId,
                request.YouTubeEmbedId,
                fileDto ?? null
            );

            await _bandService.UpdateBandAsync(updateBandRequest);
            return Ok("Band updated successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating band");
            return BadRequest();
        }
    }

    [HttpDelete("{bandId}")]
    [Authorize]
    public async Task<IActionResult> DeleteBand(string bandId)
    {
        if (string.IsNullOrEmpty(bandId))
        {
            return BadRequest("Band ID cannot be null or empty");
        }

        if (!Guid.TryParse(bandId, out var parsedBandId))
        {
            return BadRequest("Invalid Band ID format");
        }

        try
        {
            await _bandService.DeleteBandAsync(parsedBandId);
            return Ok("Band deleted successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting band with ID: {BandId}", bandId);
            return BadRequest();
        }
    }
}
