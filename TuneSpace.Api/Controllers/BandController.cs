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
            await bandService.CreateBand(request);
            var bandId = (await bandService.GetBandByName(request.Name)).Id;
            return Ok(bandId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }
}
