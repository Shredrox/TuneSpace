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
        try
        {
            var merchandise = await _merchandiseService.GetMerchandiseById(Guid.Parse(merchandiseId));
            if (merchandise == null)
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
        try
        {
            var merchandise = await _merchandiseService.GetMerchandiseByBandId(Guid.Parse(bandId));
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
        try
        {
            if (string.IsNullOrEmpty(merchandiseId))
            {
                return BadRequest("Merchandise ID cannot be null or empty");
            }
            if (!Guid.TryParse(merchandiseId, out var parsedMerchandiseId))
            {
                return BadRequest("Invalid Merchandise ID format");
            }

            var merchandise = await _merchandiseService.GetMerchandiseById(parsedMerchandiseId);

            if (merchandise?.Image == null)
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
                fileDto);

            var merchandise = await _merchandiseService.CreateMerchandise(createMerchandiseRequest);

            if (merchandise == null)
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
                fileDto);

            await _merchandiseService.UpdateMerchandise(updateMerchandiseRequest);
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
        try
        {
            await _merchandiseService.DeleteMerchandise(Guid.Parse(merchandiseId));
            return Ok("Merchandise deleted successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting merchandise with ID: {MerchandiseId}", merchandiseId);
            return BadRequest();
        }
    }
}
