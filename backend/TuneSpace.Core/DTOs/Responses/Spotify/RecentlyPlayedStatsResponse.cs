namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class RecentlyPlayedStatsResponse
{
    public List<RecentlyPlayedTrackDTO> Tracks { get; set; } = [];
    public double TotalHoursPlayed { get; set; }
    public int UniqueTracksCount { get; set; }
    public int TotalPlays { get; set; }
    public string TimePeriod { get; set; } = string.Empty;
}
