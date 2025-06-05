using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using TuneSpace.Api.Infrastructure;
using TuneSpace.Core.Common;
using TuneSpace.Core.DTOs.Requests.Auth;
using TuneSpace.Core.Enums;
using TuneSpace.Core.Exceptions;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IAuthService authService,
    IUserService userService,
    ITokenService tokenService,
    ISpotifyService spotifyService,
    IOAuthStateService oAuthStateService,
    ILogger<AuthController> logger) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IUserService _userService = userService;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ISpotifyService _spotifyService = spotifyService;
    private readonly IOAuthStateService _oAuthStateService = oAuthStateService;
    private readonly ILogger<AuthController> _logger = logger;

    [HttpGet("current-user")]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var accessToken = CookieHelper.GetAccessToken(Request);
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized("No access token provided");
            }

            var principal = await _tokenService.ValidateAccessTokenAsync(accessToken);
            if (principal is null)
            {
                return Unauthorized("Invalid access token");
            }

            var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid token claims");
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user is null)
            {
                return NotFound("User not found");
            }

            var profilePictureBase64 = user.ProfilePicture != null ?
                Convert.ToBase64String(user.ProfilePicture) : null;

            return Ok(new
            {
                id = user.Id.ToString(),
                username = user.UserName,
                email = user.Email,
                role = user.Role.ToString(),
                profilePicture = profilePictureBase64
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error during current user retrieval");
            return StatusCode(500, "An error occurred while retrieving current user information.");
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (request is null
        || string.IsNullOrEmpty(request.Name)
        || string.IsNullOrEmpty(request.Email)
        || string.IsNullOrEmpty(request.Password)
        || string.IsNullOrEmpty(request.Role))
        {
            return BadRequest("Invalid registration request.");
        }

        if (!Enum.TryParse<Roles>(request.Role, true, out var role))
        {
            return BadRequest("Invalid role specified");
        }

        try
        {
            await _authService.RegisterAsync(request.Name, request.Email, request.Password, role);
            var user = await _userService.GetUserByNameAsync(request.Name);

            if (user is null)
            {
                return NotFound("User not found");
            }

            return Ok(new { user.Id });
        }
        catch (ArgumentException e)
        {
            _logger.LogWarning(e, "Registration conflict for user: {Username}", request.Name);
            return Conflict(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error during registration for user: {Username}", request.Name);
            return StatusCode(500, "An error occurred while registering.");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (request is null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("Invalid login request.");
        }

        try
        {
            var response = await _authService.LoginAsync(request.Email, request.Password);

            var username = response.Username;
            var accessToken = response.AccessToken;
            var role = response.Role;
            var id = response.Id;

            CookieHelper.SetAuthTokens(Response, response.AccessToken, response.RefreshToken);

            return Ok(new
            {
                id,
                username,
                accessToken,
                role
            });
        }
        catch (UnauthorizedException e)
        {
            _logger.LogWarning(e, "Unauthorized login attempt for email: {Email}", request.Email);
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error during login for email: {Email}", request.Email);
            return StatusCode(500, "An error occurred while logging in.");
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var refreshToken = CookieHelper.GetRefreshToken(Request);
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Logout failed: No refresh token provided");
                return BadRequest("Refresh token is required");
            }

            await _tokenService.RevokeRefreshTokenAsync(refreshToken);

            CookieHelper.ClearAuthTokens(Response);
            Response.Cookies.Delete("SpotifyAccessToken");
            Response.Cookies.Delete("SpotifyRefreshToken");

            return NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error during logout");
            return StatusCode(500, "An error occurred during logout.");
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            var refreshToken = CookieHelper.GetRefreshToken(Request);
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Refresh token is missing");
                return BadRequest("Refresh token is required");
            }

            var user = await _tokenService.ValidateRefreshTokenAsync(refreshToken);
            if (user is null)
            {
                return Unauthorized();
            }

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            await _tokenService.SaveRefreshTokenAsync(user, newRefreshToken);

            CookieHelper.SetAuthTokens(Response, newAccessToken, newRefreshToken);

            var username = user.UserName;
            var role = user.Role.ToString().ToUpper();
            var id = user.Id;

            _logger.LogInformation("Token refreshed successfully for user: {UserId}", user.Id);
            return Ok(new
            {
                id,
                username,
                newAccessToken,
                role
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error during token refresh");
            return StatusCode(500, "An error occurred while refreshing the token.");
        }
    }

    [HttpPost("spotify-oauth")]
    public async Task<IActionResult> SpotifyOAuth([FromBody] SpotifyOAuthRequest request)
    {
        if (request is null || string.IsNullOrEmpty(request.Code))
        {
            return BadRequest("Authorization code is required.");
        }

        if (string.IsNullOrEmpty(request.State) || !_oAuthStateService.ValidateAndConsumeState(request.State))
        {
            _logger.LogWarning("Invalid or missing OAuth state parameter in Spotify OAuth request");
            return BadRequest("Invalid OAuth state parameter. This may indicate a CSRF attack.");
        }

        try
        {
            var spotifyTokens = await _spotifyService.ExchangeCodeForTokenAsync(request.Code);

            var spotifyProfile = await _spotifyService.GetUserSpotifyProfileAsync(spotifyTokens.AccessToken);

            var response = await _spotifyService.GetUserInfoAsync(spotifyTokens.AccessToken);

            var loginResponse = await _authService.ExternalLoginAsync(
                response.Id,
                response.Email,
                spotifyProfile.Username,
                ExternalProviders.Spotify,
                spotifyProfile.ProfilePicture
            );

            CookieHelper.SetAuthTokens(Response, loginResponse.AccessToken, loginResponse.RefreshToken);

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

            return Ok(new
            {
                id = loginResponse.Id,
                username = loginResponse.Username,
                accessToken = loginResponse.AccessToken,
                role = loginResponse.Role
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during Spotify OAuth authentication");
            return StatusCode(500, "An error occurred during Spotify authentication.");
        }
    }
}
