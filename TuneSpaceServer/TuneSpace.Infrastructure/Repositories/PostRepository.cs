using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

public class PostRepository(TuneSpaceDbContext context) : IPostRepository
{
    public async Task InsertPost(Post post)
    {
        context.Posts.Add(post);
        await context.SaveChangesAsync();
    }

    public async Task<Post?> GetPostById(Guid id)
    {
        return await context.Posts.FindAsync(id);
    }
}