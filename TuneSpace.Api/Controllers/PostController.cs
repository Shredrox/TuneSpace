using Microsoft.AspNetCore.Mvc;
using TuneSpace.Core.DTOs.Requests.Post;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostController(
    IPostService postService,
    ILogger<PostController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
    {
        try
        {
            await postService.CreatePost(request);
            return Created();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating post");
            return BadRequest();
        }
    }
}
