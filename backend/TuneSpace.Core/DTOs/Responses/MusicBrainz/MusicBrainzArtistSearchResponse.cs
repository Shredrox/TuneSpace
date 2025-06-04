using System.Text.Json.Serialization;

namespace TuneSpace.Core.DTOs.Responses.MusicBrainz;

public class MusicBrainzArtistSearchResponse
{
    [JsonPropertyName("artists")]
    public List<MusicBrainzArtist>? Artists { get; set; }
}

public class MusicBrainzArtist
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("tags")]
    public List<MusicBrainzTag>? Tags { get; set; }
}

public class MusicBrainzTag
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
