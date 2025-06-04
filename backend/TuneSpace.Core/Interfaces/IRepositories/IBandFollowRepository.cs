using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IRepositories;

/// <summary>
/// Repository interface for managing follow relationships between users and bands
/// </summary>
public interface IBandFollowRepository
{
    /// <summary>
    /// Retrieves a specific band follow relationship
    /// </summary>
    /// <param name="userId">ID of the user who is following</param>
    /// <param name="bandId">ID of the band being followed</param>
    /// <returns>The band follow entity if found, null otherwise</returns>
    Task<BandFollow?> GetBandFollowAsync(Guid userId, Guid bandId);

    /// <summary>
    /// Gets all users following a specific band
    /// </summary>
    /// <param name="bandId">ID of the band whose followers to retrieve</param>
    /// <returns>List of users who follow the specified band</returns>
    Task<List<User>> GetBandFollowersAsync(Guid bandId);

    /// <summary>
    /// Gets all bands that a specific user is following
    /// </summary>
    /// <param name="userId">ID of the user whose following list to retrieve</param>
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
    /// <param name="userId">ID of the user whose following count to retrieve</param>
    /// <returns>The number of bands being followed</returns>
    Task<int> GetUserFollowedBandsCountAsync(Guid userId);

    /// <summary>
    /// Checks if a user is following a specific band
    /// </summary>
    /// <param name="userId">ID of the potential follower</param>
    /// <param name="bandId">ID of the band potentially being followed</param>
    /// <returns>True if userId is following bandId, false otherwise</returns>
    Task<bool> IsFollowingBandAsync(Guid userId, Guid bandId);

    /// <summary>
    /// Creates a new band follow relationship
    /// </summary>
    /// <param name="bandFollow">The band follow entity to create</param>
    /// <returns>The created band follow entity with generated ID</returns>
    Task<BandFollow> InsertBandFollowAsync(BandFollow bandFollow);

    /// <summary>
    /// Removes a band follow relationship
    /// </summary>
    /// <param name="userId">ID of the follower to remove</param>
    /// <param name="bandId">ID of the band being followed</param>
    /// <returns>True if the relationship was found and removed, false otherwise</returns>
    Task<bool> DeleteBandFollowAsync(Guid userId, Guid bandId);
}
