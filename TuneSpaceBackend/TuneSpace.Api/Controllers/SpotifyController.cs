using Microsoft.AspNetCore.Mvc;
using TuneSpace.Core.DTOs.Requests.Spotify;
using TuneSpace.Core.DTOs.Responses.Spotify;
using TuneSpace.Core.Exceptions;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SpotifyController(
    ISpotifyService spotifyService,
    ILogger<SpotifyController> logger) : ControllerBase
{
    private readonly ISpotifyService _spotifyService = spotifyService;
    private readonly ILogger<SpotifyController> _logger = logger;

    [HttpGet("login")]
    public IActionResult SpotifyLogin()
    {
        try
        {
            var redirectUrl = _spotifyService.GetSpotifyLoginUrl();
            return Redirect(redirectUrl);

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error generating Spotify login URL");
            return BadRequest("Error generating Spotify login URL");
        }
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback(string code, string state)
    {
        try
        {
            var spotifyTokens = await _spotifyService.ExchangeCodeForToken(code);

            var tokenExpiry = DateTime.Now.AddSeconds(spotifyTokens.ExpiresIn);

            Response.Cookies.Append("SpotifyAccessToken", spotifyTokens.AccessToken, new CookieOptions
            {
                Expires = tokenExpiry,
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                Domain = "localhost",
                SameSite = SameSiteMode.None
            });

            Response.Cookies.Append("SpotifyRefreshToken", spotifyTokens.RefreshToken, new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                Domain = "localhost",
                SameSite = SameSiteMode.None
            });

            var redirectUrl = $"http://localhost:5173/?" +
                $"&tokenExpiryTime={Uri.EscapeDataString(tokenExpiry.ToString("o"))}";

            return Redirect(redirectUrl);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during Spotify callback");
            return BadRequest("Error during Spotify callback");
        }
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
            var newTokens = await _spotifyService.RefreshAccessToken(refreshToken);

            var tokenExpiry = DateTime.Now.AddSeconds(newTokens.ExpiresIn);

            Response.Cookies.Append("SpotifyAccessToken", newTokens.AccessToken, new CookieOptions
            {
                Expires = tokenExpiry,
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

            return Ok(new
            {
                spotifyTokenExpiry = tokenExpiry,
            });
        }
        catch (SpotifyApiException ex)
        {
            _logger.LogError(ex, "Error refreshing Spotify access token");
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
            var profile = await _spotifyService.GetUserSpotifyProfile(accessToken);
            var topArtists = await _spotifyService.GetUserTopArtists(accessToken);
            var topSongs = await _spotifyService.GetUserTopSongs(accessToken);

            var stats = new SpotifyStatsResponse(profile, topArtists, topSongs);

            return Ok(stats);
        }
        catch (SpotifyApiException e)
        {
            _logger.LogError(e, "Error fetching Spotify profile");
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("top-artists")]
    public async Task<IActionResult> GetUserTopArtists()
    {
        try
        {
            var accessToken = Request.Cookies["SpotifyAccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized("Access token is required");
            }

            var topArtists = await _spotifyService.GetUserTopArtists(accessToken);

            return Ok(topArtists);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching user top artists");
            return BadRequest("Error fetching user top artists");
        }
    }

    [HttpGet("top-songs")]
    public async Task<IActionResult> GetUserTopSongs()
    {
        var accessToken = Request.Cookies["SpotifyAccessToken"];
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Access token is required");
        }

        try
        {
            var topSongs = await _spotifyService.GetUserTopSongs(accessToken);
            return Ok(topSongs);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching user top songs");
            return BadRequest("Error fetching user top songs");
        }
    }

    [HttpGet("artist/{artistId}")]
    public async Task<IActionResult> GetArtist(string artistId)
    {
        var accessToken = Request.Cookies["SpotifyAccessToken"];

        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Access token is required");
        }

        try
        {
            var artist = await _spotifyService.GetArtist(accessToken, artistId);
            return Ok(artist);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching artist with ID: {ArtistId}", artistId);
            return BadRequest("Error fetching artist");
        }
    }

    [HttpGet("artists/{artistIds}")]
    public async Task<IActionResult> GetArtists(string artistIds)
    {
        var accessToken = Request.Cookies["SpotifyAccessToken"];

        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Access token is required");
        }

        try
        {
            var artists = await _spotifyService.GetSeveralArtists(accessToken, artistIds);
            return Ok(artists);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching artists with IDs: {ArtistIds}", artistIds);
            return BadRequest("Error fetching artists");
        }

    }

    [HttpGet("search/{searchTerm}")]
    public async Task<IActionResult> GetSongsBySearch(string searchTerm)
    {
        var accessToken = Request.Cookies["SpotifyAccessToken"];
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Access token is required");
        }

        if (string.IsNullOrEmpty(searchTerm))
        {
            return BadRequest("Search term cannot be empty");
        }

        try
        {
            var searchSongs = await _spotifyService.GetSongsBySearch(accessToken, searchTerm);
            return Ok(searchSongs);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching songs by search term: {SearchTerm}", searchTerm);
            return BadRequest("Error fetching songs");
        }
    }

    [HttpPost("create-playlist")]
    public async Task<IActionResult> CreatePlaylist([FromBody] CreatePlaylistRequest request)
    {
        var accessToken = Request.Cookies["SpotifyAccessToken"];
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Access token is required");
        }

        try
        {
            await _spotifyService.CreatePlaylist(accessToken, request);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating playlist");
            return BadRequest("Error creating playlist");
        }

        return Created();
    }

    [HttpGet("recently-played")]
    public async Task<IActionResult> GetUserRecentlyPlayedTracks()
    {
        var accessToken = Request.Cookies["SpotifyAccessToken"];
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Access token is required");
        }

        try
        {
            var recentlyPlayedTracks = await _spotifyService.GetUserRecentlyPlayedTracks(accessToken);
            return Ok(recentlyPlayedTracks);
        }
        catch (SpotifyApiException e)
        {
            _logger.LogError(e, "Error fetching recently played tracks");
            return StatusCode(500, e.Message);
        }
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
            var followedArtists = await _spotifyService.GetUserFollowedArtists(accessToken);
            return Ok(followedArtists);
        }
        catch (SpotifyApiException e)
        {
            _logger.LogError(e, "Error fetching followed artists");
            return StatusCode(500, e.Message);
        }
    }
}
