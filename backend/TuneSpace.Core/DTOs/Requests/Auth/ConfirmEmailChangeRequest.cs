namespace TuneSpace.Core.DTOs.Requests.Auth;

public record ConfirmEmailChangeRequest(
    string UserId,
    string Token,
    string NewEmail);
