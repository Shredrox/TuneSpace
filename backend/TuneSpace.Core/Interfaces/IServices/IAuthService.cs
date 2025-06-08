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

    /// <summary>
    /// Confirms a user's email address using the provided confirmation token.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="token">The email confirmation token.</param>
    /// <returns>A task representing the asynchronous operation indicating success or failure.</returns>
    Task<bool> ConfirmEmailAsync(string userId, string token);

    /// <summary>
    /// Confirms a user's email address and automatically logs them in.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="token">The email confirmation token.</param>
    /// <returns>A task representing the asynchronous operation that returns login credentials if successful, or null if failed.</returns>
    Task<LoginResponse?> ConfirmEmailAndLoginAsync(string userId, string token);

    /// <summary>
    /// Resends the email confirmation to a user.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ResendEmailConfirmationAsync(string email);

    /// <summary>
    /// Generates an email confirmation token for a user.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <returns>A task representing the asynchronous operation that returns the confirmation token.</returns>
    Task<string> GenerateEmailConfirmationTokenAsync(string userId);

    /// <summary>
    /// Initiates a password reset process by sending a reset email to the user.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the user is not found.</exception>
    Task RequestPasswordResetAsync(string email);

    /// <summary>
    /// Resets a user's password using the provided reset token.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="token">The password reset token.</param>
    /// <param name="newPassword">The user's new password.</param>
    /// <returns>A task representing the asynchronous operation indicating success or failure.</returns>
    Task<bool> ResetPasswordAsync(string userId, string token, string newPassword);

    /// <summary>
    /// Initiates an email change process by sending a confirmation email to the new email address.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="newEmail">The new email address.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the user is not found or email is already in use.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the user is an external provider user.</exception>
    Task RequestEmailChangeAsync(string userId, string newEmail);

    /// <summary>
    /// Confirms an email change using the provided confirmation token.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="token">The email change confirmation token.</param>
    /// <param name="newEmail">The new email address.</param>
    /// <returns>A task representing the asynchronous operation indicating success or failure.</returns>
    Task<bool> ConfirmEmailChangeAsync(string userId, string token, string newEmail);
}
