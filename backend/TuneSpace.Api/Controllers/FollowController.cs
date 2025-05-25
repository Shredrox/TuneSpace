using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TuneSpace.Core.DTOs.Responses.User;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FollowController(
    IFollowService followService,
    IUserService userService,
    ILogger<FollowController> logger) : ControllerBase
{
    private readonly IFollowService _followService = followService;
    private readonly IUserService _userService = userService;
    private readonly ILogger<FollowController> _logger = logger;

    [HttpPost("{username}")]
    [Authorize]
    public async Task<IActionResult> FollowUser(string username)
    {
        try
        {
            var user = await _userService.GetUserByName(username);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var userId = user.Id.ToString();
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                return BadRequest("Invalid user ID");
            }

            var followerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(followerIdClaim) || !Guid.TryParse(followerIdClaim, out var followerId))
            {
                return Unauthorized("Invalid or missing user ID in token");
            }

            if (followerId == parsedUserId)
            {
                return BadRequest("Users cannot follow themselves");
            }

            var result = await _followService.FollowUserAsync(followerId, parsedUserId);
            return result ? Ok("User followed successfully") : BadRequest("Failed to follow user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error following user {username}", username);
            return StatusCode(500, "An error occurred while following the user");
        }
    }

    [HttpDelete("{username}")]
    [Authorize]
    public async Task<IActionResult> UnfollowUser(string username)
    {
        try
        {
            var user = await _userService.GetUserByName(username);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var userId = user.Id.ToString();
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                return BadRequest("Invalid user ID");
            }

            var followerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(followerIdClaim) || !Guid.TryParse(followerIdClaim, out var followerId))
            {
                return Unauthorized("Invalid or missing user ID in token");
            }

            var result = await _followService.UnfollowUserAsync(followerId, parsedUserId);
            return result ? Ok("User unfollowed successfully") : BadRequest("Failed to unfollow user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unfollowing user {username}", username);
            return StatusCode(500, "An error occurred while unfollowing the user");
        }
    }

    [HttpGet("{username}/followers")]
    public async Task<ActionResult<List<UserSearchResultResponse>>> GetFollowers(string username)
    {
        try
        {
            var user = await _userService.GetUserByName(username);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var userId = user.Id.ToString();
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                return BadRequest("Invalid user ID");
            }

            var followers = await _followService.GetFollowersAsync(parsedUserId);
            return Ok(followers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting followers for user {username}", username);
            return StatusCode(500, "An error occurred while retrieving followers");
        }
    }

    [HttpGet("{username}/following")]
    public async Task<ActionResult<List<UserSearchResultResponse>>> GetFollowing(string username)
    {
        try
        {
            var user = await _userService.GetUserByName(username);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var userId = user.Id.ToString();
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                return BadRequest("Invalid user ID");
            }

            var following = await _followService.GetFollowingAsync(parsedUserId);
            return Ok(following);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting following for user {username}", username);
            return StatusCode(500, "An error occurred while retrieving following");
        }
    }

    [HttpGet("{username}/follower-count")]
    public async Task<ActionResult<int>> GetFollowerCount(string username)
    {
        try
        {
            var user = await _userService.GetUserByName(username);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var userId = user.Id.ToString();
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                return BadRequest("Invalid user ID");
            }

            var count = await _followService.GetFollowerCountAsync(parsedUserId);
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting follower count for user {username}", username);
            return StatusCode(500, "An error occurred while retrieving follower count");
        }
    }

    [HttpGet("{username}/following-count")]
    public async Task<ActionResult<int>> GetFollowingCount(string username)
    {
        try
        {
            var user = await _userService.GetUserByName(username);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var userId = user.Id.ToString();
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                return BadRequest("Invalid user ID");
            }

            var count = await _followService.GetFollowingCountAsync(parsedUserId);
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting following count for user {username}", username);
            return StatusCode(500, "An error occurred while retrieving following count");
        }
    }

    [HttpGet("{username}/is-following")]
    [Authorize]
    public async Task<ActionResult<bool>> IsFollowing(string username)
    {
        try
        {
            var user = await _userService.GetUserByName(username);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var userId = user.Id.ToString();
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
            {
                return BadRequest("Invalid user ID");
            }

            var followerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(followerIdClaim) || !Guid.TryParse(followerIdClaim, out var followerId))
            {
                return Unauthorized("Invalid or missing user ID in token");
            }

            var isFollowing = await _followService.IsFollowingAsync(followerId, parsedUserId);
            return Ok(isFollowing);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user is following {UserId}", username);
            return StatusCode(500, "An error occurred while checking follow status");
        }
    }
}
