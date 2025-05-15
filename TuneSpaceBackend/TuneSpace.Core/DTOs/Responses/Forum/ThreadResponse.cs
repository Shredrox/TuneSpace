namespace TuneSpace.Core.DTOs.Responses.Forum;

public record ThreadResponse(
    Guid Id,
    string Title,
    Guid AuthorId,
    string AuthorName,
    string AuthorAvatar,
    DateTime CreatedAt,
    DateTime LastActivityAt,
    int RepliesCount,
    int ViewsCount,
    bool IsPinned,
    bool IsLocked);
