namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class SpotifySearchResponse
{
    public SpotifyArtistSearchResults? Artists { get; set; }
    public SpotifyAlbumSearchResults? Albums { get; set; }
}

public class SpotifyArtistSearchResults
{
    public string Href { get; set; } = string.Empty;
    public List<SpotifyArtistDTO> Items { get; set; } = new();
    public int Limit { get; set; }
    public string Next { get; set; } = string.Empty;
    public int Offset { get; set; }
    public string Previous { get; set; } = string.Empty;
    public int Total { get; set; }
}

public class SpotifyAlbumSearchResults
{
    public string Href { get; set; } = string.Empty;
    public List<SpotifyAlbumDTO> Items { get; set; } = new();
    public int Limit { get; set; }
    public string Next { get; set; } = string.Empty;
    public int Offset { get; set; }
    public string Previous { get; set; } = string.Empty;
    public int Total { get; set; }
}
