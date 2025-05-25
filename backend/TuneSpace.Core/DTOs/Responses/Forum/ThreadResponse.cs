namespace TuneSpace.Core.DTOs.Responses.Forum;

public record ThreadResponse(
    Guid Id,
    string Title,
    string CategoryName,
    Guid AuthorId,
    string AuthorName,
    byte[] AuthorAvatar,
    DateTime CreatedAt,
    DateTime LastActivityAt,
    int RepliesCount,
    int ViewsCount,
    bool IsPinned,
    bool IsLocked);
