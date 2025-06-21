using System.ComponentModel.DataAnnotations;

namespace TuneSpace.Core.DTOs.Requests.MusicDiscovery;

/// <summary>
/// User preferences for music discovery without requiring Spotify connection
/// </summary>
public record UserPreferencesRequest(
    [Required]
    List<string> Genres,

    string? Location,

    List<string>? PreferredArtists = null,

    int? MaxRecommendations = 20
);
