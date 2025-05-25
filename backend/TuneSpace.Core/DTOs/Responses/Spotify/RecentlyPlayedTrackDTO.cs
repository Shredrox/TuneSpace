namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class RecentlyPlayedTrackDTO
{
    public string TrackName { get; set; }
    public string ArtistName { get; set; }
    public string ArtistId { get; set; }
    public string AlbumName { get; set; }
    public string AlbumImageUrl { get; set; }
    public DateTime PlayedAt { get; set; }
}
