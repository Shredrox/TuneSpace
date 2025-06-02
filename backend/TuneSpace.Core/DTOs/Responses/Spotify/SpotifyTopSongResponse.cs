namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class SpotifyTopSongResponse
{
    public Album Album { get; set; } = new();
    public string Name { get; set; } = string.Empty;

}

public class Album
{
    public string Name { get; set; } = string.Empty;
    public List<Artist> Artists { get; set; } = [];
    public List<SpotifyImageDTO> Images { get; set; } = [];
}

public class Artist
{
    public ExternalUrls ExternalUrls { get; set; } = new();
    public string Href { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
}

public class ExternalUrls
{
    public string Spotify { get; set; } = string.Empty;
}
