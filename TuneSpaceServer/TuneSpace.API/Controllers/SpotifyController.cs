using Microsoft.AspNetCore.Mvc;
using TuneSpace.Core.DTOs.Requests.Spotify;
using TuneSpace.Core.DTOs.Responses.Spotify;
using TuneSpace.Core.Exceptions;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SpotifyController(ISpotifyService spotifyService) : ControllerBase
{
    [HttpGet("login")]
    public IActionResult SpotifyLogin()
    {
        var redirectUrl = spotifyService.GetSpotifyLoginUrl();
        return Redirect(redirectUrl);
    }
        
    [HttpGet("callback")]
    public async Task<IActionResult> Callback(string code, string state)
    {
        var accessToken = await spotifyService.ExchangeCodeForToken(code);
        
        Response.Cookies.Append("SpotifyToken", accessToken, new CookieOptions
        {
            Expires = DateTime.Now.AddMinutes(15),
            HttpOnly = true,
            Secure = true,
            IsEssential = true,
            Domain = "localhost",
            SameSite = SameSiteMode.None
        });
        
        return Redirect("http://localhost:5173/home");
    }
    
    [HttpGet("profile")]
    public async Task<IActionResult> GetUserSpotifyProfile()
    {
        var accessToken = Request.Cookies["SpotifyToken"];
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Access token is required");
        }

        try
        {
            var profile = await spotifyService.GetUserSpotifyProfile(accessToken);
            var topArtists = await spotifyService.GetUserTopArtists(accessToken);
            var topSongs = await spotifyService.GetUserTopSongs(accessToken);

            var stats = new SpotifyStatsResponse(profile, topArtists, topSongs);
        
            return Ok(stats);
        }
        catch (SpotifyApiException e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
    [HttpGet("top-artists")]
    public async Task<IActionResult> GetUserTopArtists()
    {
        var accessToken = Request.Cookies["SpotifyToken"];
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Access token is required");
        }

        var topArtists = await spotifyService.GetUserTopArtists(accessToken);
        
        return Ok(topArtists);
    }
    
    [HttpGet("top-songs")]
    public async Task<IActionResult> GetUserTopSongs()
    {
        var accessToken = Request.Cookies["SpotifyToken"];
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Access token is required");
        }

        var topSongs = await spotifyService.GetUserTopSongs(accessToken);
        
        return Ok(topSongs);
    }
    
    [HttpGet("search/{searchTerm}")]
    public async Task<IActionResult> GetSongsBySearch(string searchTerm)
    {
        var accessToken = Request.Cookies["SpotifyToken"];
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Access token is required");
        }

        var searchSongs = await spotifyService.GetSongsBySearch(accessToken, searchTerm);
        
        return Ok(searchSongs);
    }

    [HttpPost("create-playlist")]
    public IActionResult CreatePlaylist([FromBody] CreatePlaylistRequest request)
    {
        var accessToken = Request.Cookies["SpotifyToken"];
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Access token is required");
        }
        
        spotifyService.CreatePlaylist(accessToken, request);
        
        return Created();
    }
}