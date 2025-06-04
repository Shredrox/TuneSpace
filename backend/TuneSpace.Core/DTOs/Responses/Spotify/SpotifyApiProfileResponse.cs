using System.Text.Json.Serialization;

namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class SpotifyApiProfileResponse
{
    [JsonPropertyName("display_name")]
    public string Display_Name { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("images")]
    public List<SpotifyImageDTO> Images { get; set; } = [];

    [JsonPropertyName("followers")]
    public Followers Followers { get; set; } = new();

    [JsonPropertyName("product")]
    public string Product { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}

public class Followers
{
    [JsonPropertyName("total")]
    public int Total { get; set; }
}
