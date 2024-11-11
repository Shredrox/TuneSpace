using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IRepositories;

public interface IPostRepository
{
    Task InsertPost(Post post);
    Task<Post?> GetPostById(Guid id);
}