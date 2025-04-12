using Microsoft.AspNetCore.Mvc;
using TuneSpace.Core.DTOs.Responses.User;
using TuneSpace.Core.Exceptions;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("{username}")]
    public async Task<IActionResult> GetUserByName(string username)
    {
        try
        {
            var user = await userService.GetUserByName(username);
            if (user is null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }
        catch (NotFoundException e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }

    [HttpGet("search/{search}")]
    public async Task<IActionResult> GetUsersBySearch(string search)
    {
        try
        {
            var users = await userService.SearchByName(search);
            var response = users
                .Select(user => new UserSearchResultResponse(user))
                .ToList();

            return Ok(response);
        }
        catch (NotFoundException e)
        {
            Console.WriteLine(e);
            return Ok(new List<UserSearchResultResponse>());
        }
    }
}
