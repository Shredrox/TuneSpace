using TuneSpace.Core.DTOs.Responses.MusicDiscovery;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Service for detecting and managing user location for music recommendations
/// </summary>
public interface IUserLocationService
{
    /// <summary>
    /// Detects user location based on IP address
    /// </summary>
    Task<UserLocationResponse?> DetectLocationByIpAsync(string ipAddress);

    /// <summary>
    /// Gets location information from coordinates (reverse geocoding)
    /// </summary>
    Task<UserLocationResponse?> GetLocationFromCoordinatesAsync(double latitude, double longitude);

    /// <summary>
    /// Returns a default location when detection fails
    /// </summary>
    UserLocationResponse GetDefaultLocation();
}
