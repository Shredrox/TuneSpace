using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

internal class PostRepository(TuneSpaceDbContext context) : IPostRepository
{
    async Task IPostRepository.InsertPost(Post post)
    {
        context.Posts.Add(post);
        await context.SaveChangesAsync();
    }

    async Task<Post?> IPostRepository.GetPostById(Guid id)
    {
        return await context.Posts.FindAsync(id);
    }
}