using System.Text.Json.Serialization;

namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class SpotifySearchResponse
{
    [JsonPropertyName("artists")]
    public SpotifyArtistSearchResults? Artists { get; set; }

    [JsonPropertyName("albums")]
    public SpotifyAlbumSearchResults? Albums { get; set; }
}

public class SpotifyArtistSearchResults
{
    [JsonPropertyName("href")]
    public string Href { get; set; } = string.Empty;

    [JsonPropertyName("items")]
    public List<SpotifyArtistDTO> Items { get; set; } = [];

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("next")]
    public string Next { get; set; } = string.Empty;

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("previous")]
    public string Previous { get; set; } = string.Empty;

    [JsonPropertyName("total")]
    public int Total { get; set; }
}

public class SpotifyAlbumSearchResults
{
    [JsonPropertyName("href")]
    public string Href { get; set; } = string.Empty;

    [JsonPropertyName("items")]
    public List<SpotifyAlbumDTO> Items { get; set; } = [];

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("next")]
    public string Next { get; set; } = string.Empty;

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("previous")]
    public string Previous { get; set; } = string.Empty;

    [JsonPropertyName("total")]
    public int Total { get; set; }
}
