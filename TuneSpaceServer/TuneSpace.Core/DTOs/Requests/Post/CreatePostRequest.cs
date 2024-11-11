namespace TuneSpace.Core.DTOs.Requests.Post;

public record CreatePostRequest(
    string Name,
    string Content,
    string SongID);