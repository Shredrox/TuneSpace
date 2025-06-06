namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Service interface for constructing frontend URLs
/// </summary>
public interface IUrlBuilderService
{
    /// <summary>
    /// Builds a complete URL for email confirmation
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="token">The confirmation token</param>
    /// <returns>The complete confirmation URL</returns>
    string BuildEmailConfirmationUrl(string userId, string token);

    /// <summary>
    /// Builds a complete URL for password reset
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="token">The reset token</param>
    /// <returns>The complete password reset URL</returns>
    string BuildPasswordResetUrl(string userId, string token);

    /// <summary>
    /// Builds a complete URL for email change confirmation
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="token">The change token</param>
    /// <param name="newEmail">The new email address</param>
    /// <returns>The complete email change confirmation URL</returns>
    string BuildEmailChangeConfirmationUrl(string userId, string token, string newEmail);
}
