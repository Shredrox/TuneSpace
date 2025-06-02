namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class SpotifyAlbumDTO
{
    public string AlbumType { get; set; } = string.Empty;
    public List<SpotifyArtistDTO> Artists { get; set; } = [];
    public List<string> AvailableMarkets { get; set; } = [];
    public string Href { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public List<SpotifyImageDTO> Images { get; set; } = [];
    public string Name { get; set; } = string.Empty;
    public string ReleaseDate { get; set; } = string.Empty;
    public string ReleaseDatePrecision { get; set; } = string.Empty;
    public int TotalTracks { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
}
