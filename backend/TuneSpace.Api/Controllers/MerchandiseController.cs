using Microsoft.AspNetCore.Mvc;
using TuneSpace.Api.DTOs;
using TuneSpace.Application.Common;
using TuneSpace.Core.DTOs.Requests.Merchandise;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MerchandiseController(
    IMerchandiseService merchandiseService,
    ILogger<MerchandiseController> logger) : ControllerBase
{
    private readonly IMerchandiseService _merchandiseService = merchandiseService;
    private readonly ILogger<MerchandiseController> _logger = logger;

    [HttpGet("{merchandiseId}")]
    public async Task<IActionResult> GetMerchandiseById(string merchandiseId)
    {
        if (string.IsNullOrEmpty(merchandiseId))
        {
            return BadRequest("Merchandise ID cannot be null or empty");
        }

        if (!Guid.TryParse(merchandiseId, out var parsedMerchandiseId))
        {
            return BadRequest("Invalid Merchandise ID format");
        }

        try
        {
            var merchandise = await _merchandiseService.GetMerchandiseByIdAsync(parsedMerchandiseId);
            if (merchandise is null)
            {
                return NotFound();
            }

            return Ok(merchandise);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching merchandise with ID: {MerchandiseId}", merchandiseId);
            return BadRequest();
        }
    }

    [HttpGet("band/{bandId}")]
    public async Task<IActionResult> GetMerchandiseByBandId(string bandId)
    {
        if (string.IsNullOrEmpty(bandId))
        {
            return BadRequest("Band ID cannot be null or empty");
        }

        if (!Guid.TryParse(bandId, out var parsedBandId))
        {
            return BadRequest("Invalid Band ID format");
        }

        try
        {
            var merchandise = await _merchandiseService.GetMerchandiseByBandIdAsync(parsedBandId);
            return Ok(merchandise);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching merchandise for band with ID: {BandId}", bandId);
            return BadRequest();
        }
    }

    [HttpGet("{merchandiseId}/image")]
    public async Task<IActionResult> GetMerchandiseImage(string merchandiseId)
    {
        if (string.IsNullOrEmpty(merchandiseId))
        {
            return BadRequest("Merchandise ID cannot be null or empty");
        }

        if (!Guid.TryParse(merchandiseId, out var parsedMerchandiseId))
        {
            return BadRequest("Invalid Merchandise ID format");
        }

        try
        {
            var merchandise = await _merchandiseService.GetMerchandiseByIdAsync(parsedMerchandiseId);

            if (merchandise?.Image is null)
            {
                return NotFound();
            }

            return File(merchandise.Image, "image/jpeg");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error fetching merchandise image for ID: {MerchandiseId}", merchandiseId);
            return BadRequest();
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateMerchandise([FromForm] MerchandiseCreateRequest request)
    {
        try
        {
            var fileDto = await Helpers.ConvertToFileDto(request.Image);

            var createMerchandiseRequest = new CreateMerchandiseRequest(
                Guid.Parse(request.BandId),
                request.Name,
                request.Description,
                decimal.Parse(request.Price),
                fileDto
            );

            var merchandise = await _merchandiseService.CreateMerchandiseAsync(createMerchandiseRequest);

            if (merchandise is null)
            {
                return BadRequest();
            }

            return Ok(merchandise.Id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating merchandise");
            return BadRequest();
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateMerchandise([FromForm] MerchandiseUpdateRequest request)
    {
        try
        {
            var fileDto = await Helpers.ConvertToFileDto(request.Image);

            var updateMerchandiseRequest = new UpdateMerchandiseRequest(
                Guid.Parse(request.Id),
                request.Name,
                request.Description,
                decimal.Parse(request.Price!),
                fileDto
            );

            await _merchandiseService.UpdateMerchandiseAsync(updateMerchandiseRequest);
            return Ok("Merchandise updated successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating merchandise");
            return BadRequest();
        }
    }

    [HttpDelete("{merchandiseId}")]
    public async Task<IActionResult> DeleteMerchandise(string merchandiseId)
    {
        if (string.IsNullOrEmpty(merchandiseId))
        {
            return BadRequest("Merchandise ID cannot be null or empty");
        }

        if (!Guid.TryParse(merchandiseId, out var parsedMerchandiseId))
        {
            return BadRequest("Invalid Merchandise ID format");
        }

        try
        {
            await _merchandiseService.DeleteMerchandiseAsync(parsedMerchandiseId);
            return Ok("Merchandise deleted successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting merchandise with ID: {MerchandiseId}", merchandiseId);
            return BadRequest();
        }
    }
}
