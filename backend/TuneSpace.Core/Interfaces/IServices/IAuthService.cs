using TuneSpace.Core.DTOs.Responses.Auth;
using TuneSpace.Core.Enums;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Provides authentication and registration services for users in the TuneSpace application.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="name">The user's display name.</param>
    /// <param name="email">The user's email address, which will be used for login and communication.</param>
    /// <param name="password">The user's password, which should meet security requirements.</param>
    /// <param name="role">The user's role in the system, determining their permissions.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RegisterAsync(string name, string email, string password, Roles role);

    /// <summary>
    /// Authenticates a user based on their credentials and generates authentication tokens.
    /// </summary>
    /// <param name="email">The user's registered email address.</param>
    /// <param name="password">The user's password.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns a <see cref="LoginResponse"/>
    /// containing authentication tokens and user information upon successful login.
    /// </returns>
    Task<LoginResponse> LoginAsync(string email, string password);

    /// <summary>
    /// Authenticates or registers a user using external login provider (e.g., Spotify).
    /// </summary>
    /// <param name="externalId">The external provider's user ID.</param>
    /// <param name="email">The user's email from the external provider.</param>
    /// <param name="displayName">The user's display name from the external provider.</param>
    /// <param name="provider">The name of the external provider (e.g., "Spotify").</param>
    /// <param name="profilePictureUrl">Optional profile picture URL from the external provider.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns a <see cref="LoginResponse"/>
    /// containing authentication tokens and user information.
    /// </returns>
    Task<LoginResponse> ExternalLoginAsync(string externalId, string email, string displayName, string provider, string? profilePictureUrl);

    /// <summary>
    /// Connects a Spotify account to an existing authenticated user.
    /// </summary>
    /// <param name="userId">The ID of the currently authenticated user.</param>
    /// <param name="externalId">The Spotify user ID.</param>
    /// <param name="email">The user's email from Spotify.</param>
    /// <param name="displayName">The user's display name from Spotify.</param>
    /// <param name="provider">The name of the external provider (should be "Spotify").</param>
    /// <param name="profilePictureUrl">Optional profile picture URL from Spotify.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns true if the connection was successful.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown when the Spotify account is already linked to another user.</exception>
    Task<bool> ConnectExternalAccountAsync(string userId, string externalId, string email, string displayName, string provider, string? profilePictureUrl);
}
