using Newtonsoft.Json;

namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class UserFollowedArtistsResponse
{
    [JsonProperty("artists")]
    public ArtistsContainer Artists { get; set; } = new();
}

public class ArtistsContainer
{
    [JsonProperty("href")]
    public string Href { get; set; } = string.Empty;

    [JsonProperty("limit")]
    public int Limit { get; set; }

    [JsonProperty("next")]
    public string Next { get; set; } = string.Empty;

    [JsonProperty("cursors")]
    public Cursors Cursors { get; set; } = new();

    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("items")]
    public List<ArtistItem> Items { get; set; } = new();
}

public class Cursors
{
    [JsonProperty("after")]
    public string After { get; set; } = string.Empty;
}

public class ArtistItem
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("popularity")]
    public int Popularity { get; set; }

    [JsonProperty("images")]
    public List<SpotifyImageDTO> Images { get; set; } = [];

    [JsonProperty("genres")]
    public List<string> Genres { get; set; } = [];

    [JsonProperty("followers")]
    public Followers Followers { get; set; } = new();
}
