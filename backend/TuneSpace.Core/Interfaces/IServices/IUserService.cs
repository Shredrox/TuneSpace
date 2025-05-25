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
    Task<User?> GetUserById(string id);

    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    /// <param name="name">The username to search for.</param>
    /// <returns>The user entity if found; otherwise, null.</returns>
    Task<User?> GetUserByName(string name);

    /// <summary>
    /// Retrieves a user associated with the provided refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token used to identify the user.</param>
    /// <returns>The user entity if a valid token match is found; otherwise, null.</returns>
    Task<User?> GetUserFromRefreshToken(string refreshToken);

    /// <summary>
    /// Retrieves the profile picture for a specified user.
    /// </summary>
    /// <param name="username">The username of the user whose profile picture should be retrieved.</param>
    /// <returns>The binary data of the profile picture if found; otherwise, null.</returns>
    Task<byte[]?> GetProfilePicture(string username);

    /// <summary>
    /// Searches for users whose names contain the specified search term.
    /// </summary>
    /// <param name="search">The search term to match against user names.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of user entities whose names match the search criteria.</returns>
    Task<List<User>> SearchByName(string search);

    /// <summary>
    /// Updates the refresh token for a specified user.
    /// </summary>
    /// <param name="user">The user entity whose refresh token should be updated.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateUserRefreshToken(User user);

    /// <summary>
    /// Updates the profile picture for a specified user.
    /// </summary>
    /// <param name="username">The username of the user whose profile picture should be updated.</param>
    /// <param name="profilePicture">The binary data of the profile picture.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateProfilePicture(string username, byte[] profilePicture);
}
