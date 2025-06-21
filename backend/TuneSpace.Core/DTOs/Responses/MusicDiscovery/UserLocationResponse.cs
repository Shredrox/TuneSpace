namespace TuneSpace.Core.DTOs.Responses.MusicDiscovery;

/// <summary>
/// Response containing user location information for music recommendations
/// </summary>
public record UserLocationResponse(
    string Country,
    string City,
    double? Latitude = null,
    double? Longitude = null,
    string DetectionMethod = "unknown"
);
