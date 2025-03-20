using TuneSpace.Core.Enums;

namespace TuneSpace.Core.DTOs.Responses.Auth;

public record LoginResponse(
    string Id,
    string? Username,
    UserRole Role,
    string AccessToken, 
    string RefreshToken);