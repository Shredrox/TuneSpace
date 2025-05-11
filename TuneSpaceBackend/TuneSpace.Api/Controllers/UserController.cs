using Microsoft.AspNetCore.Mvc;
using TuneSpace.Core.DTOs.Responses.User;
using TuneSpace.Core.Exceptions;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(
    IUserService userService,
    ILogger<UserController> logger) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly ILogger<UserController> _logger = logger;

    [HttpGet("{username}")]
    public async Task<IActionResult> GetUserByName(string username)
    {
        try
        {
            var user = await _userService.GetUserByName(username);
            if (user is null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }
        catch (NotFoundException e)
        {
            _logger.LogWarning(e, "User not found: {Username}", username);
            return NotFound();
        }
    }

    [HttpGet("search/{search}")]
    public async Task<IActionResult> GetUsersBySearch(string search)
    {
        if (string.IsNullOrEmpty(search))
        {
            return BadRequest("Search term cannot be empty");
        }

        try
        {
            var users = await _userService.SearchByName(search);
            var response = users
                .Select(user => new UserSearchResultResponse(user.Id, user.UserName))
                .ToList();

            return Ok(response);
        }
        catch (NotFoundException e)
        {
            _logger.LogWarning(e, "No users found for search: {Search}", search);
            return NotFound(new List<UserSearchResultResponse>());
        }
    }
}
