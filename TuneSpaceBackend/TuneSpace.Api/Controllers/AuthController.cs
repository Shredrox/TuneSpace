using Microsoft.AspNetCore.Mvc;
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
    ILogger<AuthController> logger) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IUserService _userService = userService;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ILogger<AuthController> _logger = logger;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            await _authService.Register(request.Name, request.Email, request.Password, Enum.Parse<Roles>(request.Role));
            var user = await _userService.GetUserByName(request.Name);

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
        try
        {
            var response = await _authService.Login(request.Email, request.Password);

            var username = response.Username;
            var accessToken = response.AccessToken;
            var role = response.Role;
            var id = response.Id;

            Response.Cookies.Append("AccessToken", response.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(15),
                Domain = "localhost",
                IsEssential = true,
                SameSite = SameSiteMode.None
            });

            Response.Cookies.Append("RefreshToken", response.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddHours(1),
                Domain = "localhost",
                IsEssential = true,
                SameSite = SameSiteMode.None
            });

            return Ok(new { id, username, accessToken, role });
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
            var refreshToken = Request.Cookies["RefreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Logout failed: No refresh token provided");
                return BadRequest("Refresh token is required");
            }

            var user = await _userService.GetUserFromRefreshToken(refreshToken);

            if (user is not null)
            {
                user.RefreshToken = null;
                user.RefreshTokenValidity = null;
                await _userService.UpdateUserRefreshToken(user);
                _logger.LogInformation("User logged out and refresh token cleared: {UserId}", user.Id);
            }

            Response.Cookies.Delete("AccessToken");
            Response.Cookies.Delete("RefreshToken");

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
            var refreshToken = Request.Cookies["RefreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Refresh token is missing");
                return BadRequest("Refresh token is required");
            }

            var user = await _userService.GetUserFromRefreshToken(refreshToken);

            if (user is null)
            {
                return NotFound();
            }

            var username = user.UserName;
            var role = user.Role.ToString().ToUpper();
            var id = user.Id;

            if (user is null)
            {
                _logger.LogWarning("Invalid refresh token provided");
                return BadRequest("Invalid refresh token");
            }

            var newAccessToken = _tokenService.CreateAccessToken(user);
            var newRefreshToken = await _tokenService.CreateRefreshToken(user);

            Response.Cookies.Append("AccessToken", newAccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(15),
                Domain = "localhost",
                IsEssential = true,
                SameSite = SameSiteMode.None
            });

            Response.Cookies.Append("RefreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddHours(24),
                Domain = "localhost",
                IsEssential = true,
                SameSite = SameSiteMode.None
            });

            _logger.LogInformation("Token refreshed successfully for user: {UserId}", user.Id);
            return Ok(new { id, username, newAccessToken, role });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error during token refresh");
            return StatusCode(500, "An error occurred while refreshing the token.");
        }
    }
}
