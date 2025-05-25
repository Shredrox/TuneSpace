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
    Task Register(string name, string email, string password, Roles role);

    /// <summary>
    /// Authenticates a user based on their credentials and generates authentication tokens.
    /// </summary>
    /// <param name="email">The user's registered email address.</param>
    /// <param name="password">The user's password.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns a <see cref="LoginResponse"/>
    /// containing authentication tokens and user information upon successful login.
    /// </returns>
    Task<LoginResponse> Login(string email, string password);
}
