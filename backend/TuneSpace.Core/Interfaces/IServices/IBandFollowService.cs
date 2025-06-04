using TuneSpace.Core.DTOs.Responses.User;
using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Service interface for managing band follow relationships and business logic
/// </summary>
public interface IBandFollowService
{
    /// <summary>
    /// Retrieves a specific band follow relationship
    /// </summary>
    /// <param name="userId">ID of the user following the band</param>
    /// <param name="bandId">ID of the band being followed</param>
    /// <returns>The band follow entity if found, null otherwise</returns>
    Task<BandFollow?> GetBandFollowAsync(Guid userId, Guid bandId);

    /// <summary>
    /// Gets all users following a specific band
    /// </summary>
    /// <param name="bandId">ID of the band whose followers to retrieve</param>
    /// <returns>List of users who follow the specified band</returns>
    Task<List<UserSearchResultResponse>> GetBandFollowersAsync(Guid bandId);

    /// <summary>
    /// Gets all bands that a specific user is following
    /// </summary>
    /// <param name="userId">ID of the user whose followed bands to retrieve</param>
    /// <returns>List of bands being followed by the specified user</returns>
    Task<List<Band>> GetUserFollowedBandsAsync(Guid userId);

    /// <summary>
    /// Gets the count of followers for a specific band
    /// </summary>
    /// <param name="bandId">ID of the band whose follower count to retrieve</param>
    /// <returns>The number of followers</returns>
    Task<int> GetBandFollowerCountAsync(Guid bandId);

    /// <summary>
    /// Gets the count of bands that a specific user is following
    /// </summary>
    /// <param name="userId">ID of the user whose followed bands count to retrieve</param>
    /// <returns>The number of bands being followed</returns>
    Task<int> GetUserFollowedBandsCountAsync(Guid userId);

    /// <summary>
    /// Checks if a user is following a band
    /// </summary>
    /// <param name="userId">ID of the potential follower</param>
    /// <param name="bandId">ID of the band potentially being followed</param>
    /// <returns>True if user is following the band, false otherwise</returns>
    Task<bool> IsFollowingBandAsync(Guid userId, Guid bandId);

    /// <summary>
    /// Creates a new band follow relationship
    /// </summary>
    /// <param name="userId">ID of the user following the band</param>
    /// <param name="bandId">ID of the band to follow</param>
    /// <returns>True if the follow was created, false if already exists</returns>
    Task<bool> FollowBandAsync(Guid userId, Guid bandId);

    /// <summary>
    /// Removes a band follow relationship
    /// </summary>
    /// <param name="userId">ID of the user unfollowing the band</param>
    /// <param name="bandId">ID of the band to unfollow</param>
    /// <returns>True if the relationship was found and removed, false otherwise</returns>
    Task<bool> UnfollowBandAsync(Guid userId, Guid bandId);
}
