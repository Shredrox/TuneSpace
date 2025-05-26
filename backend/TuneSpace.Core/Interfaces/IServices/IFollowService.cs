using TuneSpace.Core.DTOs.Responses.User;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Service interface for managing user follow relationships
/// </summary>
public interface IFollowService
{
    /// <summary>
    /// Gets all users following a specific user
    /// </summary>
    /// <param name="userId">ID of the user whose followers to retrieve</param>
    /// <returns>List of users who follow the specified user</returns>
    Task<List<UserSearchResultResponse>> GetFollowersAsync(Guid userId);

    /// <summary>
    /// Gets all users that a specific user is following
    /// </summary>
    /// <param name="followerId">ID of the user whose following list to retrieve</param>
    /// <returns>List of users being followed by the specified user</returns>
    Task<List<UserSearchResultResponse>> GetFollowingAsync(Guid followerId);

    /// <summary>
    /// Gets the count of followers for a specific user
    /// </summary>
    /// <param name="userId">ID of the user whose follower count to retrieve</param>
    /// <returns>The number of followers</returns>
    Task<int> GetFollowerCountAsync(Guid userId);

    /// <summary>
    /// Gets the count of users that a specific user is following
    /// </summary>
    /// <param name="followerId">ID of the user whose following count to retrieve</param>
    /// <returns>The number of users being followed</returns>
    Task<int> GetFollowingCountAsync(Guid followerId);

    /// <summary>
    /// Checks if a user is following another user
    /// </summary>
    /// <param name="followerId">ID of the potential follower</param>
    /// <param name="userId">ID of the user potentially being followed</param>
    /// <returns>True if followerId is following userId, false otherwise</returns>
    Task<bool> IsFollowingAsync(Guid followerId, Guid userId);

    /// <summary>
    /// Creates a follow relationship between two users
    /// </summary>
    /// <param name="followerId">ID of the user who wants to follow another user</param>
    /// <param name="userId">ID of the user to be followed</param>
    /// <returns>True if the follow operation was successful, false otherwise</returns>
    Task<bool> FollowUserAsync(Guid followerId, Guid userId);

    /// <summary>
    /// Removes a follow relationship between two users
    /// </summary>
    /// <param name="followerId">ID of the user who wants to unfollow another user</param>
    /// <param name="userId">ID of the user to be unfollowed</param>
    /// <returns>True if the unfollow operation was successful, false otherwise</returns>
    Task<bool> UnfollowUserAsync(Guid followerId, Guid userId);
}
