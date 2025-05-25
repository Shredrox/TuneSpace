using Microsoft.Extensions.Logging;
using TuneSpace.Core.DTOs.Requests.Merchandise;
using TuneSpace.Core.DTOs.Responses.Merchandise;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Application.Services;

internal class MerchandiseService(
    IMerchandiseRepository merchandiseRepository,
    IBandRepository bandRepository,
    ILogger<MerchandiseService> logger) : IMerchandiseService
{
    private readonly IMerchandiseRepository _merchandiseRepository = merchandiseRepository;
    private readonly IBandRepository _bandRepository = bandRepository;
    private readonly ILogger<MerchandiseService> _logger = logger;

    async Task<Merchandise?> IMerchandiseService.CreateMerchandise(CreateMerchandiseRequest request)
    {
        try
        {
            _logger.LogInformation("Creating new merchandise item for band {BandId}", request.BandId);

            var band = await _bandRepository.GetBandById(request.BandId);
            if (band == null)
            {
                _logger.LogWarning("Band with ID {BandId} not found when creating merchandise", request.BandId);
                return null;
            }

            var merchandise = new Merchandise
            {
                BandId = request.BandId,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Image = request.Image?.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _merchandiseRepository.CreateMerchandiseAsync(merchandise);
            _logger.LogInformation("Merchandise {MerchandiseName} created successfully with ID {MerchandiseId}", merchandise.Name, merchandise.Id);
            return merchandise;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating merchandise for band {BandId}", request.BandId);
            throw;
        }
    }

    async Task<MerchandiseResponse?> IMerchandiseService.GetMerchandiseById(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting merchandise with ID {MerchandiseId}", id);
            var merchandise = await _merchandiseRepository.GetMerchandiseByIdAsync(id);
            if (merchandise == null)
            {
                _logger.LogWarning("Merchandise with ID {MerchandiseId} not found", id);
                return null;
            }

            return new MerchandiseResponse(
                merchandise.Id.ToString(),
                merchandise.BandId.ToString(),
                merchandise.Name,
                merchandise.Description ?? string.Empty,
                merchandise.Price,
                merchandise.Image,
                merchandise.CreatedAt,
                merchandise.UpdatedAt
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving merchandise with ID {MerchandiseId}", id);
            throw;
        }
    }

    async Task<IEnumerable<MerchandiseResponse>> IMerchandiseService.GetMerchandiseByBandId(Guid bandId)
    {
        try
        {
            _logger.LogInformation("Getting merchandise for band with ID {BandId}", bandId);
            var merchandises = await _merchandiseRepository.GetAllMerchandisesByBandIdAsync(bandId);

            return merchandises.Select(m => new MerchandiseResponse(
                m.Id.ToString(),
                m.BandId.ToString(),
                m.Name,
                m.Description ?? string.Empty,
                m.Price,
                m.Image,
                m.CreatedAt,
                m.UpdatedAt
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving merchandise for band with ID {BandId}", bandId);
            throw;
        }
    }

    async Task IMerchandiseService.UpdateMerchandise(UpdateMerchandiseRequest request)
    {
        try
        {
            _logger.LogInformation("Updating merchandise with ID {Id}", request.Id);

            var merchandise = await _merchandiseRepository.GetMerchandiseByIdAsync(request.Id);
            if (merchandise == null)
            {
                _logger.LogWarning("Merchandise with ID {Id} not found for update", request.Id);
                return;
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                merchandise.Name = request.Name;
            }

            if (!string.IsNullOrEmpty(request.Description))
            {
                merchandise.Description = request.Description;
            }

            if (request.Price.HasValue)
            {
                merchandise.Price = request.Price.Value;
            }

            if (request.Image != null)
            {
                merchandise.Image = request.Image.Content;
            }

            merchandise.UpdatedAt = DateTime.UtcNow;

            await _merchandiseRepository.UpdateMerchandiseAsync(merchandise);
            _logger.LogInformation("Merchandise with ID {Id} updated successfully", request.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating merchandise with ID {Id}", request.Id);
            throw;
        }
    }

    async Task IMerchandiseService.DeleteMerchandise(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting merchandise with ID {Id}", id);
            var result = await _merchandiseRepository.DeleteMerchandiseAsync(id);
            if (!result)
            {
                _logger.LogWarning("Merchandise with ID {Id} not found for deletion", id);
            }
            else
            {
                _logger.LogInformation("Merchandise with ID {Id} deleted successfully", id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting merchandise with ID {Id}", id);
            throw;
        }
    }
}
