namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class SpotifyApiProfileResponse
{
    public string Display_Name { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public List<SpotifyImageDTO> Images { get; set; } = [];
    public Followers Followers { get; set; } = new();
    public string Product { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class Followers
{
    public int Total { get; set; }
}
