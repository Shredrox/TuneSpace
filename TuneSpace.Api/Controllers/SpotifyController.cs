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
        var spotifyTokens = await spotifyService.ExchangeCodeForToken(code);
        
        Response.Cookies.Append("SpotifyAccessToken", spotifyTokens.AccessToken, new CookieOptions
        {
            Expires = DateTime.Now.AddHours(1),
            HttpOnly = true,
            Secure = true,
            IsEssential = true,
            Domain = "localhost",
            SameSite = SameSiteMode.None
        });

        Response.Cookies.Append("SpotifyRefreshToken", spotifyTokens.RefreshToken, new CookieOptions
        {
            Expires = DateTime.Now.AddHours(1),
            HttpOnly = true,
            Secure = true,
            IsEssential = true,
            Domain = "localhost",
            SameSite = SameSiteMode.None
        });
        
        var redirectUrl = $"http://localhost:5173/";
        return Redirect(redirectUrl);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["SpotifyRefreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized("Refresh token is required");
        }
        
        try
        {
            var newTokens = await spotifyService.RefreshAccessToken(refreshToken);
            
            Response.Cookies.Append("SpotifyAccessToken", newTokens.AccessToken, new CookieOptions
            {
                Expires = DateTime.Now.AddHours(1),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                Domain = "localhost",
                SameSite = SameSiteMode.None
            });
            
            if (!string.IsNullOrEmpty(newTokens.RefreshToken) && newTokens.RefreshToken != refreshToken)
            {
                Response.Cookies.Append("SpotifyRefreshToken", newTokens.RefreshToken, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30),
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    Domain = "localhost",
                    SameSite = SameSiteMode.None
                });
            }
            
            return Ok(new { AccessToken = newTokens.AccessToken });
        }
        catch (SpotifyApiException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("profile")]
    public async Task<IActionResult> GetUserSpotifyProfile()
    {
        var accessToken = Request.Cookies["SpotifyAccessToken"];
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
        var accessToken = Request.Cookies["SpotifyAccessToken"];
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
        var accessToken = Request.Cookies["SpotifyAccessToken"];
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Access token is required");
        }

        var topSongs = await spotifyService.GetUserTopSongs(accessToken);
        
        return Ok(topSongs);
    }

    [HttpGet("artist/{artistId}")]
    public async Task<IActionResult> GetArtist(string artistId)
    {
        var accessToken = Request.Cookies["SpotifyAccessToken"];
        
        if (string.IsNullOrEmpty(accessToken))
        {
          return Unauthorized("Access token is required");
        }   

        var artist = await spotifyService.GetArtist(accessToken, artistId); 
        return Ok(artist);
    }
    
    [HttpGet("artists/{artistIds}")]
    public async Task<IActionResult> GetArtists(string artistIds)
    {
        var accessToken = Request.Cookies["SpotifyAccessToken"];
        
        if (string.IsNullOrEmpty(accessToken))
        {
          return Unauthorized("Access token is required");
        }   

        var artists = await spotifyService.GetSeveralArtists(accessToken, artistIds); 
        return Ok(artists);
    }

    [HttpGet("search/{searchTerm}")]
    public async Task<IActionResult> GetSongsBySearch(string searchTerm)
    {
        var accessToken = Request.Cookies["SpotifyAccessToken"];
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
        var accessToken = Request.Cookies["SpotifyAccessToken"];
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Access token is required");
        }
        
        spotifyService.CreatePlaylist(accessToken, request);
        
        return Created();
    }

    [HttpGet("followed-artists")]
    public async Task<IActionResult> GetUserFollowedArtists()
    {
        var accessToken = Request.Cookies["SpotifyAccessToken"];
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Access token is required");
        }

        try
        {
            var followedArtists = await spotifyService.GetUserFollowedArtists(accessToken);
            return Ok(followedArtists);
        }
        catch (SpotifyApiException e)
        {
            return StatusCode(500, e.Message);
        }
    }
}