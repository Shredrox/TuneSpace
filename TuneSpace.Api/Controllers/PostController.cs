using Microsoft.AspNetCore.Mvc;
using TuneSpace.Core.DTOs.Requests.Post;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostController(IPostService postService) : ControllerBase
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
            Console.WriteLine(e);
            return BadRequest();
        }
    }
}
