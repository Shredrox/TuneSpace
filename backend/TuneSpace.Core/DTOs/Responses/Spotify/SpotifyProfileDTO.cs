namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class SpotifyProfileDTO
{
    public string Username { get; set; } = string.Empty;
    public string ProfilePicture { get; set; } = string.Empty;
    public int FollowerCount { get; set; }
    public string SpotifyPlan { get; set; } = string.Empty;
}
