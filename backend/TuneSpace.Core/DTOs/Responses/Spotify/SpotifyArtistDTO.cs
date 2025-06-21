using System.Text.Json.Serialization;

namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class SpotifyArtistDTO
{
    [JsonPropertyName("followers")]
    public Followers Followers { get; set; } = new();

    [JsonPropertyName("genres")]
    public List<string> Genres { get; set; } = [];

    [JsonPropertyName("href")]
    public string Href { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("images")]
    public List<SpotifyImageDTO> Images { get; set; } = [];

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("popularity")]
    public int Popularity { get; set; }

    [JsonPropertyName("external_urls")]
    public SpotifyExternalUrls ExternalUrls { get; set; } = new();
}

public class SpotifyExternalUrls
{
    [JsonPropertyName("spotify")]
    public string Spotify { get; set; } = string.Empty;
}
