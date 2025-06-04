using System.Security.Claims;
using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Provides token generation services for authentication and authorization in the TuneSpace application.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a short-lived access token for the specified user.
    /// </summary>
    /// <param name="user">The user entity for which to generate the access token.</param>
    /// <returns>A JWT access token string.</returns>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Generates a long-lived refresh token that can be used to obtain new access tokens.
    /// </summary>
    /// <returns>The refresh token string.</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Saves the specified refresh token for the user, associating it with their account.
    /// </summary>
    /// <param name="user">The user entity for which to save the refresh token.</param>
    /// <param name="refreshToken">The refresh token string to save.</param>
    /// <returns>A task representing the asynchronous operation that returns the saved refresh token string.</returns>
    Task<string> SaveRefreshTokenAsync(User user, string refreshToken);

    /// <summary>
    /// Validates the specified access token and returns the associated claims if the token is valid.
    /// </summary>
    /// <param name="token">The access token to validate.</param>
    /// <returns>A ClaimsPrincipal representing the claims in the token, or null if the token is invalid.</returns>
    Task<ClaimsPrincipal?> ValidateAccessTokenAsync(string token);

    /// <summary>
    /// Validates the specified refresh token and returns the associated claims if the token is valid.
    /// </summary>
    /// <param name="token">The refresh token to validate.</param>
    /// <returns>A user entity if the token is valid, or null if the token is invalid or expired.</returns>
    Task<User?> ValidateRefreshTokenAsync(string token);

    /// <summary>
    /// Revokes the specified refresh token, making it invalid for future use.
    /// </summary>
    /// <param name="token">The refresh token to revoke.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RevokeRefreshTokenAsync(string token);
}
