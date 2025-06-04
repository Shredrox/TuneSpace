namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class RecentlyPlayedTrackDTO
{
    public string TrackName { get; set; } = string.Empty;
    public string ArtistName { get; set; } = string.Empty;
    public string ArtistId { get; set; } = string.Empty;
    public string AlbumName { get; set; } = string.Empty;
    public string AlbumImageUrl { get; set; } = string.Empty;
    public DateTime PlayedAt { get; set; }
    public int DurationMs { get; set; }
    public double DurationMinutes => DurationMs / 60000.0;
}
