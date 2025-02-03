using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Core.Services;

public class TagService(ITagRepository tagRepository) : ITagService
{
    public async Task CreateTag(string name)
    {
        var tag = new Tag
        {
            Name = name
        };

        await tagRepository.InsertTag(tag);
    }

    public async Task<IEnumerable<Tag>> GetAllTags()
    {
        return await tagRepository.GetAllTags();
    }
}