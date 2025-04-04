using Microsoft.AspNetCore.Mvc;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MusicDiscoveryController(IMusicDiscoveryService musicDiscoveryService) : ControllerBase
{
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

            var recommendations = await musicDiscoveryService.GetBandRecommendationsAsync(
                accessToken, 
                genresList, 
                location ?? "");

            return Ok(recommendations);
       }
       catch (Exception e)
       {
            Console.WriteLine(e);
            return BadRequest(e.Message);
       }
    }
}