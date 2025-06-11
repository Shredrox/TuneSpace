using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Provides user management services for handling user-related operations in the TuneSpace application.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The user entity if found; otherwise, null.</returns>
    Task<User?> GetUserByIdAsync(string id);

    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    /// <param name="name">The username to search for.</param>
    /// <returns>The user entity if found; otherwise, null.</returns>
    Task<User?> GetUserByNameAsync(string name);

    /// <summary>
    /// Retrieves the profile picture for a specified user.
    /// </summary>
    /// <param name="username">The username of the user whose profile picture should be retrieved.</param>
    /// <returns>The binary data of the profile picture if found; otherwise, null.</returns>
    Task<byte[]?> GetProfilePictureAsync(string username);

    /// <summary>
    /// Searches for users whose names contain the specified search term.
    /// </summary>
    /// <param name="search">The search term to match against user names.</param>
    /// <param name="currentUserId">The ID of the current user to exclude from results.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of user entities whose names match the search criteria.</returns>
    Task<List<User>> SearchByNameAsync(string search, string? currentUserId);

    /// <summary>
    /// Updates the profile picture for a specified user.
    /// </summary>
    /// <param name="username">The username of the user whose profile picture should be updated.</param>
    /// <param name="profilePicture">The binary data of the profile picture.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateProfilePictureAsync(string username, byte[] profilePicture);

    /// <summary>
    /// Retrieves users who have been active within the specified number of days.
    /// </summary>
    /// <param name="daysBack">The number of days to look back for activity.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of active user IDs.</returns>
    Task<List<string>> GetActiveUserIdsAsync(int daysBack);

    /// <summary>
    /// Updates the last active date for a user to the current UTC time.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateUserActivityAsync(string userId);
}
