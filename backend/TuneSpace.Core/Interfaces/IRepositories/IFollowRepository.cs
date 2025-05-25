using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IRepositories;

/// <summary>
/// Repository interface for managing follow relationships between users
/// </summary>
public interface IFollowRepository
{
    /// <summary>
    /// Creates a new follow relationship
    /// </summary>
    /// <param name="follow">The follow entity to create</param>
    /// <returns>The created follow entity with generated ID</returns>
    Task<Follow> CreateFollowAsync(Follow follow);

    /// <summary>
    /// Retrieves a specific follow relationship
    /// </summary>
    /// <param name="followerId">ID of the user who is following</param>
    /// <param name="userId">ID of the user being followed</param>
    /// <returns>The follow entity if found, null otherwise</returns>
    Task<Follow?> GetFollowAsync(Guid followerId, Guid userId);

    /// <summary>
    /// Gets all users following a specific user
    /// </summary>
    /// <param name="userId">ID of the user whose followers to retrieve</param>
    /// <returns>List of users who follow the specified user</returns>
    Task<List<User>> GetFollowersAsync(Guid userId);

    /// <summary>
    /// Gets all users that a specific user is following
    /// </summary>
    /// <param name="followerId">ID of the user whose following list to retrieve</param>
    /// <returns>List of users being followed by the specified user</returns>
    Task<List<User>> GetFollowingAsync(Guid followerId);

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
    /// Removes a follow relationship
    /// </summary>
    /// <param name="followerId">ID of the follower to remove</param>
    /// <param name="userId">ID of the user being followed</param>
    /// <returns>True if the relationship was found and removed, false otherwise</returns>
    Task<bool> DeleteFollowAsync(Guid followerId, Guid userId);
}
