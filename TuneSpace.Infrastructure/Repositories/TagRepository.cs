using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

internal class TagRepository(TuneSpaceDbContext context) : ITagRepository
{
    async Task ITagRepository.InsertTag(Tag tag)
    {
        context.Tags.Add(tag);
        await context.SaveChangesAsync();
    }

    async Task<IEnumerable<Tag>> ITagRepository.GetAllTags()
    {
        return await context.Tags.ToListAsync();
    }
}
