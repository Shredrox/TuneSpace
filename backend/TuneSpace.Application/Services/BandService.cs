using Microsoft.Extensions.Logging;
using TuneSpace.Core.DTOs.Requests.Band;
using TuneSpace.Core.DTOs.Responses.Band;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Enums;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Application.Services;

internal class BandService(
    IBandRepository bandRepository,
    IUserRepository userRepository,
    ILogger<BandService> logger) : IBandService
{
    private readonly IBandRepository _bandRepository = bandRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILogger<BandService> _logger = logger;

    async Task<Band?> IBandService.CreateBand(CreateBandRequest request)
    {
        try
        {
            _logger.LogInformation("Creating new band with name: {BandName}", request.Name);

            var user = await _userRepository.GetUserById(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found when creating band", request.UserId);
                return null;
            }

            var city = request.Location.Split(',')[1].Trim();
            var country = request.Location.Split(',')[0].Trim();

            user.Role = Roles.BandAdmin;
            await _userRepository.UpdateUser(user);
            _logger.LogInformation("User {UserName} updated to BandAdmin role", user.UserName);

            var band = new Band
            {
                Name = request.Name,
                Genre = request.Genre,
                Description = request.Description,
                Country = country,
                City = city,
                CoverImage = request.Picture?.Content,
                Members = [user],
            };

            await _bandRepository.InsertBand(band);
            _logger.LogInformation("Band {BandName} created successfully with ID {BandId}", band.Name, band.Id);
            return band;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating band with name {BandName}", request.Name);
            throw;
        }
    }

    async Task<Band?> IBandService.GetBandById(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting band with ID {BandId}", id);
            var band = await _bandRepository.GetBandById(id);
            if (band == null)
            {
                _logger.LogWarning("Band with ID {BandId} not found", id);
                return null;
            }
            return band;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving band with ID {BandId}", id);
            throw;
        }
    }

    async Task<Band?> IBandService.GetBandByName(string name)
    {
        try
        {
            _logger.LogInformation("Getting band with name {BandName}", name);
            var band = await _bandRepository.GetBandByName(name);
            if (band == null)
            {
                _logger.LogWarning("Band with name {BandName} not found", name);
            }
            return band;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving band with name {BandName}", name);
            throw;
        }
    }

    async Task<BandResponse?> IBandService.GetBandByUserId(string id)
    {
        try
        {
            _logger.LogInformation("Getting band for user with ID {UserId}", id);
            var band = await _bandRepository.GetBandByUserId(id);
            if (band == null)
            {
                _logger.LogWarning("No band found for user with ID {UserId}", id);
                return null;
            }

            return new BandResponse(
                band.Id.ToString(),
                band.Name,
                band.Description ?? string.Empty,
                band.Genre,
                band.Country ?? string.Empty,
                band.City ?? string.Empty,
                band.CoverImage ?? [],
                band.SpotifyId,
                band.YouTubeEmbedId,
                band.Members?.Select(m => new MemberResponse(
                    m.Id.ToString(),
                    m.UserName ?? string.Empty,
                    "",
                    m.ProfilePicture
                )).ToList()
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving band for user with ID {UserId}", id);
            throw;
        }
    }

    async Task<byte[]?> IBandService.GetBandImage(Guid bandId)
    {
        try
        {
            _logger.LogInformation("Getting image for band with ID {BandId}", bandId);
            var band = await _bandRepository.GetBandById(bandId);
            if (band == null)
            {
                _logger.LogWarning("Band with ID {BandId} not found when retrieving image", bandId);
                return null;
            }
            return band.CoverImage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving image for band with ID {BandId}", bandId);
            throw;
        }
    }

    async Task IBandService.UpdateBand(UpdateBandRequest request)
    {
        try
        {
            _logger.LogInformation("Updating band with ID {Id}", request.Id);

            var band = await _bandRepository.GetBandById(request.Id);
            if (band == null)
            {
                _logger.LogWarning("Band with ID {Id} not found for update", request.Id);
                return;
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                band.Name = request.Name;
            }

            if (!string.IsNullOrEmpty(request.Description))
            {
                band.Description = request.Description;
            }

            if (!string.IsNullOrEmpty(request.Genre))
            {
                band.Genre = request.Genre;
            }

            if (!string.IsNullOrEmpty(request.SpotifyId))
            {
                band.SpotifyId = request.SpotifyId;
            }

            if (!string.IsNullOrEmpty(request.YouTubeEmbedId))
            {
                band.YouTubeEmbedId = request.YouTubeEmbedId;
            }

            if (request.Picture != null)
            {
                band.CoverImage = request.Picture.Content;
            }

            await _bandRepository.UpdateBand(band);
            _logger.LogInformation("Band with ID {Id} updated successfully", request.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating band with ID {Id}", request.Id);
            throw;
        }
    }

    async Task IBandService.AddMemberToBand(Guid userId, Guid bandId)
    {
        try
        {
            _logger.LogInformation("Adding user with ID {UserId} to band with ID {BandId}", userId, bandId);
            var band = await _bandRepository.GetBandById(bandId);
            if (band == null)
            {
                _logger.LogWarning("Band with ID {BandId} not found when adding member", bandId);
                return;
            }

            var user = await _userRepository.GetUserById(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found when adding to band", userId);
                return;
            }

            band.Members.Add(user);
            await _bandRepository.UpdateBand(band);
            _logger.LogInformation("User with ID {UserId} added to band with ID {BandId} successfully", userId, bandId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user with ID {UserId} to band with ID {BandId}", userId, bandId);
            throw;
        }
    }

    async Task IBandService.DeleteBand(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting band with ID {Id}", id);
            await _bandRepository.DeleteBand(id);
            _logger.LogInformation("Band with ID {Id} deleted successfully", id);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Band with ID {Id} not found for deletion", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting band with ID {Id}", id);
            throw;
        }
    }
}
