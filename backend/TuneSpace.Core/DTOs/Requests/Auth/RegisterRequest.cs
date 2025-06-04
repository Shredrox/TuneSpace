namespace TuneSpace.Core.DTOs.Requests.Auth;

public record RegisterRequest(
    string Name,
    string Email,
    string Password,
    string Role);
