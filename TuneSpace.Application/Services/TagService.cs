using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Application.Services;

internal class TagService(ITagRepository tagRepository) : ITagService
{
    async Task ITagService.CreateTag(string name)
    {
        var tag = new Tag
        {
            Name = name
        };

        await tagRepository.InsertTag(tag);
    }

    async Task<IEnumerable<Tag>> ITagService.GetAllTags()
    {
        return await tagRepository.GetAllTags();
    }
}
