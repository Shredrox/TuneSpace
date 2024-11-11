using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IServices;

public interface ITagService
{
    Task CreateTag(string name);
    Task<IEnumerable<Tag>> GetAllTags();
}