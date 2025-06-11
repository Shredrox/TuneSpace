namespace TuneSpace.Core.DTOs.Requests.Auth;

public record ResetPasswordRequest(
    string UserId,
    string Token,
    string NewPassword);
