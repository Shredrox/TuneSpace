using Microsoft.AspNetCore.Http;
using TuneSpace.Core.DTOs;

namespace TuneSpace.Application.Common;

public static class Helpers
{
    public static async Task<FileDto?> ConvertToFileDto(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return null;
        }

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        return new FileDto
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            Content = memoryStream.ToArray()
        };
    }

    public static string GenerateRandomString(int length)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string([.. Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)])]);
    }

    public static (DateTime startTime, DateTime endTime, string periodName) GetTimeRangeForPeriod(string period)
    {
        var now = DateTime.UtcNow;
        DateTime startTime;
        DateTime endTime;
        string periodName;

        switch (period.ToLower())
        {
            case "today":
                startTime = now.Date;
                endTime = now;
                periodName = "Today";
                break;

            case "this-week":
                int daysSinceMonday = ((int)now.DayOfWeek + 6) % 7;
                startTime = now.Date.AddDays(-daysSinceMonday);
                endTime = now;
                periodName = "This Week";
                break;

            default:
                throw new ArgumentException($"Unsupported period: {period}");
        }

        // Ensure we don't go beyond Spotify's 24-hour limit for recently played
        // var maxStartTime = now.AddHours(-24);
        // if (startTime < maxStartTime)
        // {
        //     startTime = maxStartTime;
        // }

        return (startTime, endTime, periodName);
    }
}
