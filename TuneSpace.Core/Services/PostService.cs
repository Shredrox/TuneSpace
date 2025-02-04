using TuneSpace.Core.DTOs.Requests.Post;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Core.Services;

internal class PostService(IPostRepository postRepository) : IPostService
{
    async Task IPostService.CreatePost(CreatePostRequest request)
    {
        var newPost = new Post
        {
            Title = request.Name,
            Content = request.Content
        };
        
        await postRepository.InsertPost(newPost);
    }
}