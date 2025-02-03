namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class SpotifyApiProfileResponse
{
    public string Display_Name { get; set; }
    public string Id { get; set; }
    public List<SpotifyImageDTO> Images { get; set; }
    public Followers Followers { get; set; }
    public string Product { get; set; }
    public string Email { get; set; }
}

public class Followers
{
    public int Total { get; set; }
}