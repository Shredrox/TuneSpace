using TuneSpace.Core.Enums;

namespace TuneSpace.Core.DTOs.Responses.Auth;

public record LoginResponse(
    string Id,
    string? Username,
    Roles Role,
    string AccessToken,
    string RefreshToken);
