using System.Text.Json.Serialization;

namespace TuneSpace.Core.DTOs.Responses.LastFm;

public class LastFmArtistInfoResponse
{
    [JsonPropertyName("artist")]
    public LastFmArtist? Artist { get; set; }
}

public class LastFmArtist
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("stats")]
    public LastFmStats? Stats { get; set; }

    [JsonPropertyName("image")]
    public List<LastFmImage>? Images { get; set; }

    [JsonPropertyName("tags")]
    public LastFmTags? Tags { get; set; }
}

public class LastFmStats
{
    [JsonPropertyName("playcount")]
    public string? PlayCount { get; set; }

    [JsonPropertyName("listeners")]
    public string? Listeners { get; set; }
}

public class LastFmImage
{
    [JsonPropertyName("#text")]
    public string? Text { get; set; }

    [JsonPropertyName("size")]
    public string? Size { get; set; }
}

public class LastFmTags
{
    [JsonPropertyName("tag")]
    public List<LastFmTag>? Tag { get; set; }
}

public class LastFmTag
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
