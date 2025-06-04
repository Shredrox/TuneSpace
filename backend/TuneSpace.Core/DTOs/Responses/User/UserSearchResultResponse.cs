namespace TuneSpace.Core.DTOs.Responses.User;

public record UserSearchResultResponse(
    Guid Id,
    string Name,
    byte[] ProfilePicture);
