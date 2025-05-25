using Microsoft.AspNetCore.Mvc;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MusicDiscoveryController(
    IMusicDiscoveryService musicDiscoveryService,
    ILogger<MusicDiscoveryController> logger) : ControllerBase
{
    private readonly IMusicDiscoveryService _musicDiscoveryService = musicDiscoveryService;
    private readonly ILogger<MusicDiscoveryController> _logger = logger;

    [HttpGet("recommendations")]
    public async Task<IActionResult> GetRecommendations([FromQuery] string? genres, [FromQuery] string? location = null)
    {
        try
        {
            var accessToken = Request.Cookies["SpotifyAccessToken"];

            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized("Access token is required");
            }

            var genresList = string.IsNullOrEmpty(genres)
                ? new List<string>()
                : genres.Split(',').ToList();

            var recommendations = await _musicDiscoveryService.GetBandRecommendationsAsync(
                accessToken,
                genresList,
                location ?? "");

            return Ok(recommendations);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching band recommendations");
            return BadRequest(e.Message);
        }
    }
}
