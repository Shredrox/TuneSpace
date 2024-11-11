using Microsoft.AspNetCore.Mvc;
using TuneSpace.Core.DTOs.Requests.Auth;
using TuneSpace.Core.Exceptions;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            await authService.Register(request.Name, request.Email, request.Password);
            return Created();
        }
        catch (ArgumentException e)
        {
            return Conflict(e.Message);
        }
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await authService.Login(request.Email, request.Password);
            
            var username = response.Username;
            var accessToken = response.AccessToken;
            var role = "USER";
            
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
            
            return Ok(new { username, accessToken, role });
        }
        catch (UnauthorizedException e)
        {
            return Unauthorized(e.Message);
        }
    }
}