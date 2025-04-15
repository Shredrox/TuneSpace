namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class SpotifyTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
}
