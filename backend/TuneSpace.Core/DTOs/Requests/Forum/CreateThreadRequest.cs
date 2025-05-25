namespace TuneSpace.Core.DTOs.Requests.Forum;

public record CreateThreadRequest(
    string Title,
    string Content,
    Guid CategoryId);
