namespace TuneSpace.Core.DTOs.Requests.Auth;

public record SpotifyOAuthRequest(
    string Code,
    string? State);
