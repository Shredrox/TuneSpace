using TuneSpace.Core.DTOs.Requests.Post;

namespace TuneSpace.Core.Interfaces.IServices;

public interface IPostService
{
    Task CreatePost(CreatePostRequest post);
}
