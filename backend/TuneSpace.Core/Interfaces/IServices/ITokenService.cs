using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Provides token generation services for authentication and authorization in the TuneSpace application.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Creates a short-lived access token for the specified user.
    /// </summary>
    /// <param name="user">The user entity for which to generate the access token.</param>
    /// <returns>A JWT access token string.</returns>
    string CreateAccessToken(User user);

    /// <summary>
    /// Creates a long-lived refresh token for the specified user that can be used to obtain new access tokens.
    /// </summary>
    /// <param name="user">The user entity for which to generate the refresh token.</param>
    /// <returns>A task representing the asynchronous operation that returns the refresh token string.</returns>
    Task<string> CreateRefreshToken(User user);
}
