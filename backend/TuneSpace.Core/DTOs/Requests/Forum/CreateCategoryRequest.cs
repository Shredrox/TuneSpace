namespace TuneSpace.Core.DTOs.Requests.Forum;

public record CreateCategoryRequest(
    string Name,
    string Description,
    string? IconName);
