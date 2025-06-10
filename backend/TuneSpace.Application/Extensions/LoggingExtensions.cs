using System.Text.RegularExpressions;

namespace TuneSpace.Application.Extensions;

/// <summary>
/// Provides extension methods for safe logging to prevent log injection attacks.
/// </summary>
public static partial class LoggingExtensions
{
    /// <summary>
    /// Sanitizes user input for safe logging by removing control characters and limiting length.
    /// </summary>
    /// <param name="input">The input string to sanitize.</param>
    /// <param name="maxLength">Maximum allowed length (default: 100).</param>
    /// <returns>Sanitized string safe for logging.</returns>
    public static string SanitizeForLogging(this string? input, int maxLength = 100)
    {
        if (string.IsNullOrEmpty(input))
        {
            return "[empty]";
        }

        var sanitized = ControlCharacterRegex().Replace(input, "");

        sanitized = sanitized.Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' ');

        if (sanitized.Length > maxLength)
        {
            sanitized = sanitized[..maxLength] + "...";
        }

        if (string.IsNullOrWhiteSpace(sanitized) && !string.IsNullOrWhiteSpace(input))
        {
            return "[redacted]";
        }

        return sanitized;
    }

    /// <summary>
    /// Sanitizes an email address for logging, keeping only basic structure for debugging.
    /// </summary>
    /// <param name="email">The email address to sanitize.</param>
    /// <returns>Sanitized email safe for logging.</returns>
    public static string SanitizeEmailForLogging(this string? email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return "[empty]";
        }

        var sanitized = email.SanitizeForLogging(50);

        var atIndex = sanitized.IndexOf('@');
        if (atIndex > 0 && atIndex < sanitized.Length - 1)
        {
            var localPart = sanitized[..atIndex];
            var domain = sanitized[(atIndex + 1)..];

            if (localPart.Length > 0)
            {
                return $"{localPart[0]}***({localPart.Length})@{domain}";
            }
        }

        return "[email-format-invalid]";
    }

    /// <summary>
    /// Sanitizes a user ID for logging.
    /// </summary>
    /// <param name="userId">The user ID to sanitize.</param>
    /// <returns>Sanitized user ID safe for logging.</returns>
    public static string SanitizeUserIdForLogging(this string? userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return "[empty]";
        }

        if (Guid.TryParse(userId, out _) && userId.Length == 36)
        {
            return $"{userId[..8]}...{userId[^4..]}";
        }

        return userId.SanitizeForLogging(20);
    }

    [GeneratedRegex(@"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F-\x9F]")]
    private static partial Regex ControlCharacterRegex();
}
