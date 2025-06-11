using System.Text.Json.Serialization;

namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class SpotifyTopArtistResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("popularity")]
    public int Popularity { get; set; }

    [JsonPropertyName("followers")]
    public Followers Followers { get; set; } = new();

    [JsonPropertyName("images")]
    public List<SpotifyImageDTO> Images { get; set; } = [];
}
