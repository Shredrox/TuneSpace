﻿using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> GetBandById(string bandId)
    {
        try
        {
            var band = await _bandService.GetBandById(Guid.Parse(bandId));
            return Ok(band);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching band with ID: {BandId}", bandId);
            return BadRequest();
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetBandByUserId(string userId)
    {
        try
        {
            var band = await _bandService.GetBandByUserId(userId);
            return Ok(band);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching band for user with ID: {UserId}", userId);
            return BadRequest();
        }
    }

    [HttpGet("{bandId}/image")]
    public async Task<IActionResult> GetBandImage(string bandId)
    {
        try
        {
            if (string.IsNullOrEmpty(bandId))
            {
                return BadRequest("Band ID cannot be null or empty");
            }
            if (!Guid.TryParse(bandId, out var parsedBandId))
            {
                return BadRequest("Invalid Band ID format");
            }

            var imageData = await _bandService.GetBandImage(parsedBandId);

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
                fileDto ?? null);

            var band = await _bandService.CreateBand(createBandRequest);

            if (band == null)
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

    [HttpPut("update")]
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
                fileDto ?? null);

            await _bandService.UpdateBand(updateBandRequest);
            return Ok("Band updated successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating band");
            return BadRequest();
        }
    }

    [HttpDelete("{bandId}")]
    public async Task<IActionResult> DeleteBand(string bandId)
    {
        try
        {
            await _bandService.DeleteBand(Guid.Parse(bandId));
            return Ok("Band deleted successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting band with ID: {BandId}", bandId);
            return BadRequest();
        }
    }
}
