using Microsoft.Extensions.Logging;
using TuneSpace.Core.DTOs.Responses.User;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Application.Services;

internal class FollowService(
    IFollowRepository followRepository,
    IUserRepository userRepository,
    ILogger<FollowService> logger) : IFollowService
{
    private readonly IFollowRepository _followRepository = followRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILogger<FollowService> _logger = logger;

    async Task<List<UserSearchResultResponse>> IFollowService.GetFollowersAsync(Guid userId)
    {
        try
        {
            var followers = await _followRepository.GetFollowersAsync(userId);
            return [.. followers.Select(u => new UserSearchResultResponse(u.Id, u.UserName ?? string.Empty, u.ProfilePicture ?? []))];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting followers for user {UserId}", userId);
            throw;
        }
    }

    async Task<List<UserSearchResultResponse>> IFollowService.GetFollowingAsync(Guid followerId)
    {
        try
        {
            var following = await _followRepository.GetFollowingAsync(followerId);
            return [.. following.Select(u => new UserSearchResultResponse(u.Id, u.UserName ?? string.Empty, u.ProfilePicture ?? []))];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting following users for user {FollowerId}", followerId);
            throw;
        }
    }

    async Task<int> IFollowService.GetFollowerCountAsync(Guid userId)
    {
        try
        {
            return await _followRepository.GetFollowerCountAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting follower count for user {UserId}", userId);
            throw;
        }
    }

    async Task<int> IFollowService.GetFollowingCountAsync(Guid followerId)
    {
        try
        {
            return await _followRepository.GetFollowingCountAsync(followerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting following count for user {FollowerId}", followerId);
            throw;
        }
    }

    async Task<bool> IFollowService.IsFollowingAsync(Guid followerId, Guid userId)
    {
        try
        {
            return await _followRepository.IsFollowingAsync(followerId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {FollowerId} is following user {UserId}", followerId, userId);
            throw;
        }
    }

    async Task<bool> IFollowService.FollowUserAsync(Guid followerId, Guid userId)
    {
        try
        {
            var existingFollow = await _followRepository.GetFollowAsync(followerId, userId);
            if (existingFollow != null)
            {
                _logger.LogInformation("User {FollowerId} is already following user {UserId}", followerId, userId);
                return false;
            }

            var follower = await _userRepository.GetUserByIdAsync(followerId.ToString());
            var user = await _userRepository.GetUserByIdAsync(userId.ToString());

            if (follower == null || user == null)
            {
                _logger.LogWarning("Follower {FollowerId} or user {UserId} not found", followerId, userId);
                return false;
            }

            var follow = new Follow
            {
                FollowerId = followerId,
                UserId = userId,
                Timestamp = DateTime.UtcNow
            };

            await _followRepository.InsertFollowAsync(follow);
            _logger.LogInformation("User {FollowerId} is now following user {UserId}", followerId, userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error following user {UserId} by {FollowerId}", userId, followerId);
            throw;
        }
    }

    async Task<bool> IFollowService.UnfollowUserAsync(Guid followerId, Guid userId)
    {
        try
        {
            var result = await _followRepository.DeleteFollowAsync(followerId, userId);
            if (result)
            {
                _logger.LogInformation("User {FollowerId} unfollowed user {UserId}", followerId, userId);
            }
            else
            {
                _logger.LogWarning("User {FollowerId} was not following user {UserId}", followerId, userId);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unfollowing user {UserId} by {FollowerId}", userId, followerId);
            throw;
        }
    }
}
