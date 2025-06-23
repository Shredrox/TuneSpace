namespace TuneSpace.Api.Extensions;

public static class LoggingExtensions
{
    /// <summary>
    /// Sanitizes user input to prevent log forging attacks by removing newlines and other control characters
    /// </summary>
    /// <param name="input">The user input to sanitize</param>
    /// <returns>Sanitized string safe for logging</returns>
    public static string SanitizeForLogging(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        return input
            .Replace(Environment.NewLine, "")
            .Replace("\n", "")
            .Replace("\r", "")
            .Replace("\t", "")
            .Replace("\b", "")
            .Replace("\f", "")
            .Replace("\v", "");
    }
}
