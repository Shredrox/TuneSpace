using System.Text.Json.Serialization;

namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class SpotifyTopArtistResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("popularity")]
    public int Popularity { get; set; }

    [JsonPropertyName("images")]
    public List<SpotifyImageDTO> Images { get; set; } = [];
}
