using Microsoft.Extensions.Logging;
using TuneSpace.Core.DTOs.Responses.User;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Application.Services;

internal class BandFollowService(
    IBandFollowRepository bandFollowRepository,
    IBandRepository bandRepository,
    IUserRepository userRepository,
    ILogger<BandFollowService> logger) : IBandFollowService
{
    private readonly IBandFollowRepository _bandFollowRepository = bandFollowRepository;
    private readonly IBandRepository _bandRepository = bandRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILogger<BandFollowService> _logger = logger;

    async Task<BandFollow?> IBandFollowService.GetBandFollowAsync(Guid userId, Guid bandId)
    {
        try
        {
            _logger.LogInformation("Getting band follow relationship");
            return await _bandFollowRepository.GetBandFollowAsync(userId, bandId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting band follow relationship");
            throw;
        }
    }

    async Task<List<UserSearchResultResponse>> IBandFollowService.GetBandFollowersAsync(Guid bandId)
    {
        try
        {
            _logger.LogInformation("Getting followers for band {BandId}", bandId);
            var followers = await _bandFollowRepository.GetBandFollowersAsync(bandId);
            return [.. followers.Select(u => new UserSearchResultResponse(u.Id, u.UserName ?? string.Empty, u.ProfilePicture ?? []))];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting followers for band {BandId}", bandId);
            throw;
        }
    }

    async Task<List<Band>> IBandFollowService.GetUserFollowedBandsAsync(Guid userId)
    {
        try
        {
            _logger.LogInformation("Getting followed bands for user");
            return await _bandFollowRepository.GetUserFollowedBandsAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting followed bands for user");
            throw;
        }
    }

    async Task<int> IBandFollowService.GetBandFollowerCountAsync(Guid bandId)
    {
        try
        {
            _logger.LogInformation("Getting follower count for band {BandId}", bandId);
            return await _bandFollowRepository.GetBandFollowerCountAsync(bandId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting follower count for band {BandId}", bandId);
            throw;
        }
    }

    async Task<int> IBandFollowService.GetUserFollowedBandsCountAsync(Guid userId)
    {
        try
        {
            _logger.LogInformation("Getting followed bands count for user");
            return await _bandFollowRepository.GetUserFollowedBandsCountAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting followed bands count for user");
            throw;
        }
    }

    async Task<bool> IBandFollowService.IsFollowingBandAsync(Guid userId, Guid bandId)
    {
        try
        {
            _logger.LogInformation("Checking if user is following band");
            return await _bandFollowRepository.IsFollowingBandAsync(userId, bandId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user is following band");
            throw;
        }
    }

    async Task<bool> IBandFollowService.FollowBandAsync(Guid userId, Guid bandId)
    {
        try
        {
            _logger.LogInformation("User attempting to follow band");

            var existingFollow = await _bandFollowRepository.GetBandFollowAsync(userId, bandId);
            if (existingFollow != null)
            {
                _logger.LogInformation("User is already following band");
                return false;
            }

            var user = await _userRepository.GetUserByIdAsync(userId.ToString());
            var band = await _bandRepository.GetBandByIdAsync(bandId);

            if (user is null)
            {
                _logger.LogWarning("User not found when attempting to follow band");
                return false;
            }

            if (band is null)
            {
                _logger.LogWarning("Band not found when user attempting to follow");
                return false;
            }

            var bandFollow = new BandFollow
            {
                UserId = userId,
                BandId = bandId,
                Timestamp = DateTime.UtcNow
            };

            await _bandFollowRepository.InsertBandFollowAsync(bandFollow);
            _logger.LogInformation("User {UserId} successfully followed band {BandId}", userId, bandId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when user {UserId} attempting to follow band {BandId}", userId, bandId);
            throw;
        }
    }

    async Task<bool> IBandFollowService.UnfollowBandAsync(Guid userId, Guid bandId)
    {
        try
        {
            _logger.LogInformation("User {UserId} attempting to unfollow band {BandId}", userId, bandId);
            var result = await _bandFollowRepository.DeleteBandFollowAsync(userId, bandId);

            if (result)
            {
                _logger.LogInformation("User {UserId} successfully unfollowed band {BandId}", userId, bandId);
            }
            else
            {
                _logger.LogWarning("Band follow relationship between user {UserId} and band {BandId} not found for removal", userId, bandId);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when user {UserId} attempting to unfollow band {BandId}", userId, bandId);
            throw;
        }
    }
}
