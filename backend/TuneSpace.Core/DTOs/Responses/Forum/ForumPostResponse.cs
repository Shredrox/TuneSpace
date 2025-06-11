namespace TuneSpace.Core.DTOs.Responses.Forum;

public record ForumPostResponse(
    Guid Id,
    string Content,
    Guid AuthorId,
    string AuthorName,
    byte[] AuthorAvatar,
    string AuthorRole,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int LikesCount,
    bool UserHasLiked,
    Guid? ParentPostId = null,
    List<ForumPostResponse>? Replies = null);
