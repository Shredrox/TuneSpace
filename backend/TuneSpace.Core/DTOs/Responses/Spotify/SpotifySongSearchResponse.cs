using System.Text.Json.Serialization;

namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class SpotifySongSearchResponse
{
    [JsonPropertyName("tracks")]
    public SearchItems Tracks { get; set; } = new();
}

public class SearchItems
{
    [JsonPropertyName("items")]
    public List<SearchSong> Items { get; set; } = [];
}

public class SearchSong
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("artists")]
    public List<Artist> Artists { get; set; } = [];

    [JsonPropertyName("album")]
    public SearchSongAlbum Album { get; set; } = new();
}

public class SongSearchImageDTO
{
    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }
}

public class SearchSongAlbum
{
    [JsonPropertyName("images")]
    public List<SongSearchImageDTO> Images { get; set; } = [];
}
