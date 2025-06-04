using System.Text.Json.Serialization;

namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class UserFollowedArtistsResponse
{
    [JsonPropertyName("artists")]
    public ArtistsContainer Artists { get; set; } = new();
}

public class ArtistsContainer
{
    [JsonPropertyName("href")]
    public string Href { get; set; } = string.Empty;

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("next")]
    public string Next { get; set; } = string.Empty;

    [JsonPropertyName("cursors")]
    public Cursors Cursors { get; set; } = new();

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("items")]
    public List<ArtistItem> Items { get; set; } = new();
}

public class Cursors
{
    [JsonPropertyName("after")]
    public string After { get; set; } = string.Empty;
}

public class ArtistItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("popularity")]
    public int Popularity { get; set; }

    [JsonPropertyName("images")]
    public List<SpotifyImageDTO> Images { get; set; } = [];

    [JsonPropertyName("genres")]
    public List<string> Genres { get; set; } = [];

    [JsonPropertyName("followers")]
    public Followers Followers { get; set; } = new();
}
