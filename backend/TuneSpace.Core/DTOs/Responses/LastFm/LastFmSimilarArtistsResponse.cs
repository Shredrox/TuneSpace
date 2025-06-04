using System.Text.Json.Serialization;

namespace TuneSpace.Core.DTOs.Responses.LastFm;

public class LastFmSimilarArtistsResponse
{
    [JsonPropertyName("similarartists")]
    public LastFmSimilarArtists? SimilarArtists { get; set; }
}

public class LastFmSimilarArtists
{
    [JsonPropertyName("artist")]
    public List<LastFmSimilarArtist>? Artists { get; set; }
}

public class LastFmSimilarArtist
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
