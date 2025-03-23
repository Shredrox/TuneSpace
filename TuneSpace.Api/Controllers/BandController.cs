using Microsoft.AspNetCore.Mvc;
using TuneSpace.Core.DTOs.Requests.Band;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BandController(IBandService bandService) : ControllerBase
{
    [HttpGet("{bandId}")]
    public async Task<IActionResult> GetBandById(string bandId)
    {
        try
        {
            var band = await bandService.GetBandById(Guid.Parse(bandId));
            return Ok(band);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetBandByUserId(string userId)
    {
        try
        {
            var band = await bandService.GetBandByUserId(userId);
            return Ok(band);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }

    [HttpGet("{bandId}/image")]
    public async Task<IActionResult> GetBandImage(string bandId)
    {
        var imageData = await bandService.GetBandImage(Guid.Parse(bandId));
            
        if (imageData is null)
        {
            return NotFound();
        }
            
        return File(imageData, "image/jpeg");
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateBand([FromForm] CreateBandRequest request)
    {
        try
        {
            var band = await bandService.CreateBand(request);

            if(band == null)
            {
                return BadRequest();
            }

            return Ok(band.Id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateBand([FromBody] UpdateBandRequest request)
    {
        try
        {
            await bandService.UpdateBand(request);
            return Ok("Band updated successfully");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }

    [HttpPut("delete/{bandId}")]
    public async Task<IActionResult> DeleteBand(string bandId)
    {
        try
        {
            await bandService.DeleteBand(Guid.Parse(bandId));
            return Ok("Band deleted successfully");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }
}
