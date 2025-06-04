namespace TuneSpace.Core.DTOs.Requests.Forum;

public record CreatePostRequest(
    string Content,
    Guid ThreadId);
