namespace TuneSpace.Core.DTOs.Responses.Forum;

public record CategoryResponse(
    Guid Id,
    string Name,
    string Description,
    string? IconName,
    int TotalThreads,
    int TotalPosts);
