using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TuneSpace.Api.Extensions;
using TuneSpace.Core.DTOs.Responses.Spotify;
using TuneSpace.Core.Exceptions;
using TuneSpace.Core.Interfaces.IServices;
using TuneSpace.Infrastructure.Options;

namespace TuneSpace.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SpotifyController(
    ISpotifyService spotifyService,
    ILogger<SpotifyController> logger,
    IUrlBuilderService urlBuilderService,
    IOptions<SecurityOptions> securityOptions) : ControllerBase
{
    private readonly ISpotifyService _spotifyService = spotifyService;
    private readonly ILogger<SpotifyController> _logger = logger;
    private readonly IUrlBuilderService _urlBuilderService = urlBuilderService;
    private readonly SecurityOptions _securityOptions = securityOptions.Value;

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

    [HttpGet("connect")]
    public IActionResult SpotifyConnect()
    {
        try
        {
            var redirectUrl = _spotifyService.GetSpotifyLoginUrl("connect");
            return Redirect(redirectUrl);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error generating Spotify connect URL");
            return BadRequest("Error generating Spotify connect URL");
        }
    }

    [HttpGet("callback")]
    public IActionResult Callback(string code, string state)
    {
        if (string.IsNullOrEmpty(state))
        {
            _logger.LogWarning("Invalid or missing OAuth state parameter");
            return BadRequest("Invalid OAuth state parameter.");
        }

        if (string.IsNullOrEmpty(code))
        {
            _logger.LogWarning("Missing authorization code in Spotify callback");
            return BadRequest("Missing authorization code");
        }

        try
        {
            var flowType = "login";
            if (state.Contains(':'))
            {
                var parts = state.Split(':', 2);
                if (parts.Length == 2 && (parts[0] == "login" || parts[0] == "connect"))
                {
                    flowType = parts[0];
                }
            }

            var redirectUrl = flowType switch
            {
                "connect" => _urlBuilderService.BuildSpotifyConnectCallbackUrl(code, state),
                _ => _urlBuilderService.BuildSpotifyLoginCallbackUrl(code, state)
            };

            if (!IsValidRedirectUrl(redirectUrl))
            {
                _logger.LogWarning("Invalid redirect URL detected: {RedirectUrl}", LoggingExtensions.SanitizeForLogging(redirectUrl));
                return BadRequest("Invalid redirect URL");
            }

            return Redirect(redirectUrl);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during Spotify callback");
            return BadRequest("Error during Spotify callback");
        }
    }

    [HttpGet("connection-status")]
    public IActionResult GetConnectionStatus()
    {
        var accessToken = Request.Cookies["SpotifyAccessToken"];
        var refreshToken = Request.Cookies["SpotifyRefreshToken"];

        var isConnected = !string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken);

        return Ok(new
        {
            isConnected,
            hasTokens = isConnected
        });
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
            var profile = await _spotifyService.GetUserSpotifyProfileAsync(accessToken);
            var topArtists = await _spotifyService.GetUserTopArtistsAsync(accessToken);
            var topSongs = await _spotifyService.GetUserTopSongsAsync(accessToken);

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
        var accessToken = Request.Cookies["SpotifyAccessToken"];

        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Access token is required");
        }

        try
        {
            var topArtists = await _spotifyService.GetUserTopArtistsAsync(accessToken);
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
            var topSongs = await _spotifyService.GetUserTopSongsAsync(accessToken);
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
            var artist = await _spotifyService.GetArtistAsync(accessToken, artistId);
            return Ok(artist);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching artist with ID: {ArtistId}", LoggingExtensions.SanitizeForLogging(artistId));
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
            var artists = await _spotifyService.GetSeveralArtistsAsync(accessToken, artistIds);
            return Ok(artists);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching artists with IDs: {ArtistIds}", LoggingExtensions.SanitizeForLogging(artistIds));
            return BadRequest("Error fetching artists");
        }

    }

    [HttpGet("listening-stats/today")]
    public async Task<IActionResult> GetTodayListeningStats()
    {
        // TODO: Refactor
        // Currently limits to last 50 tracks played today (in the last 24 hours)
        // Should aggregate the stats for the day

        var accessToken = Request.Cookies["SpotifyAccessToken"];

        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Access token is required");
        }

        try
        {
            var stats = await _spotifyService.GetListeningStatsForPeriodAsync(accessToken, "today");
            return Ok(stats);
        }
        catch (SpotifyApiException e)
        {
            _logger.LogError(e, "Error fetching today's listening stats");
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("listening-stats/this-week")]
    public async Task<IActionResult> GetThisWeekListeningStats()
    {
        // TODO: Implement this endpoint to fetch listening stats for the current week
        // Currently, it's returning the same stats as the "today" endpoint
        // as Spoitfy API does not provide a direct way to get weekly stats.
        // May need to aggregate daily stats for the week.

        var accessToken = Request.Cookies["SpotifyAccessToken"];

        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Access token is required");
        }

        try
        {
            var stats = await _spotifyService.GetListeningStatsForPeriodAsync(accessToken, "this-week");
            return Ok(stats);
        }
        catch (SpotifyApiException e)
        {
            _logger.LogError(e, "Error fetching this week's listening stats");
            return StatusCode(500, e.Message);
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
            var searchSongs = await _spotifyService.GetSongsBySearchAsync(accessToken, searchTerm);
            return Ok(searchSongs);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching songs by search term: {SearchTerm}", LoggingExtensions.SanitizeForLogging(searchTerm));
            return BadRequest("Error fetching songs");
        }
    }

    [HttpGet("search-artists/{searchTerm}")]
    public async Task<IActionResult> SearchArtists(string searchTerm)
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
            var searchResponse = await _spotifyService.SearchAsync(accessToken, searchTerm, "artist", 10);
            return Ok(searchResponse.Artists?.Items ?? []);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error searching for artists with term: {SearchTerm}", LoggingExtensions.SanitizeForLogging(searchTerm));
            return BadRequest("Error searching for artists");
        }
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
            var recentlyPlayedTracks = await _spotifyService.GetUserRecentlyPlayedTracksAsync(accessToken);
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
            var followedArtists = await _spotifyService.GetUserFollowedArtistsAsync(accessToken);
            return Ok(followedArtists);
        }
        catch (SpotifyApiException e)
        {
            _logger.LogError(e, "Error fetching followed artists");
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["SpotifyRefreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Ok(new
            {
                isConnected = false
            });
        }

        try
        {
            var newTokens = await _spotifyService.RefreshAccessTokenAsync(refreshToken);

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
                isConnected = true
            });
        }
        catch (SpotifyApiException ex)
        {
            _logger.LogError(ex, "Error refreshing Spotify access token");

            Response.Cookies.Delete("SpotifyAccessToken");
            Response.Cookies.Delete("SpotifyRefreshToken");

            return Ok(new
            {
                isConnected = false,
                error = "Spotify connection expired. Please reconnect."
            });
        }
    }

    private bool IsValidRedirectUrl(string redirectUrl)
    {
        if (string.IsNullOrEmpty(redirectUrl))
            return false;

        try
        {
            var uri = new Uri(redirectUrl, UriKind.Absolute);

            var allowedHosts = _securityOptions.AllowedRedirectHosts;

            return allowedHosts.Any(host =>
                string.Equals(uri.Host, host, StringComparison.OrdinalIgnoreCase));
        }
        catch (UriFormatException)
        {
            return false;
        }
    }
}
