using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Defines the contract for email services in the TuneSpace application.
/// Provides methods for sending various types of emails related to user authentication,
/// account management, and user engagement.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email confirmation message to a user to verify their email address.
    /// This is typically used during the user registration process.
    /// </summary>
    /// <param name="user">The user who needs to confirm their email address.</param>
    /// <param name="confirmationToken">The unique token used to verify the email confirmation.</param>
    /// <param name="callbackUrl">The URL that the user should visit to complete the email confirmation.</param>
    /// <returns>A task representing the asynchronous email sending operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the email service is not properly configured.</exception>
    Task SendEmailConfirmationAsync(User user, string confirmationToken, string callbackUrl);

    /// <summary>
    /// Sends a welcome email to a newly registered user.
    /// This email typically contains onboarding information and getting started instructions.
    /// </summary>
    /// <param name="user">The user who should receive the welcome email.</param>
    /// <returns>A task representing the asynchronous email sending operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the user parameter is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the email service is not properly configured.</exception>
    Task SendWelcomeEmailAsync(User user);

    /// <summary>
    /// Sends a password reset email to a user who has requested to reset their password.
    /// The email contains a secure link that allows the user to set a new password.
    /// </summary>
    /// <param name="user">The user who requested the password reset.</param>
    /// <param name="resetToken">The unique token used to authorize the password reset operation.</param>
    /// <param name="callbackUrl">The URL that the user should visit to reset their password.</param>
    /// <returns>A task representing the asynchronous email sending operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the email service is not properly configured.</exception>
    Task SendPasswordResetEmailAsync(User user, string resetToken, string callbackUrl);

    /// <summary>
    /// Sends an email change confirmation message when a user requests to change their email address.
    /// The confirmation email is sent to the new email address to verify ownership.
    /// </summary>
    /// <param name="user">The user who is requesting to change their email address.</param>
    /// <param name="newEmail">The new email address that the user wants to use.</param>
    /// <param name="confirmationToken">The unique token used to verify the email change request.</param>
    /// <param name="callbackUrl">The URL that the user should visit to complete the email change confirmation.</param>
    /// <returns>A task representing the asynchronous email sending operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the new email address is invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the email service is not properly configured.</exception>
    Task SendEmailChangeConfirmationAsync(User user, string newEmail, string confirmationToken, string callbackUrl);
}
