using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IRepositories;

public interface ITagRepository
{
    Task InsertTag(Tag tag);
    Task<IEnumerable<Tag>> GetAllTags();
}