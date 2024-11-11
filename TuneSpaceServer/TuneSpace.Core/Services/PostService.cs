using TuneSpace.Core.DTOs.Requests.Post;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Core.Services;

public class PostService(IPostRepository postRepository) : IPostService
{
    public async Task CreatePost(CreatePostRequest request)
    {
        var newPost = new Post
        {
            Title = request.Name,
            Content = request.Content
        };
        
        await postRepository.InsertPost(newPost);
    }
}