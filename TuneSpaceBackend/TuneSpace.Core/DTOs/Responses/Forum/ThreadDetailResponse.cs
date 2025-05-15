namespace TuneSpace.Core.DTOs.Responses.Forum;

public record ThreadDetailResponse(
    Guid Id,
    string Title,
    CategorySummaryResponse Category,
    Guid CategoryId,
    string CategoryName,
    DateTime CreatedAt,
    bool IsPinned,
    bool IsLocked,
    List<ForumPostResponse> Posts);
