namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class SpotifyTopSongResponse
{
    public Album Album { get; set; }
    public string Name { get; set; }

}

public class Album
{
    public string Name { get; set; }
    public List<Artist> Artists { get; set; }
    public List<SpotifyImageDTO> Images { get; set; }
}

public class Artist
{
    public ExternalUrls ExternalUrls { get; set; }
    public string Href { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Uri { get; set; }
}

public class ExternalUrls
{
    public string Spotify { get; set; }
}
