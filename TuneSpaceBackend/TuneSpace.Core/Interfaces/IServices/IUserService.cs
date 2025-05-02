using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Provides user management services for handling user-related operations in the TuneSpace application.
/// </summary>
public interface IUserService
{
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
    /// Searches for users whose names contain the specified search term.
    /// </summary>
    /// <param name="search">The search term to match against user names.</param>
    /// <returns>A list of matching usernames.</returns>
    Task<List<string>> SearchByName(string search);

    /// <summary>
    /// Updates the refresh token for a specified user.
    /// </summary>
    /// <param name="user">The user entity whose refresh token should be updated.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateUserRefreshToken(User user);
}
