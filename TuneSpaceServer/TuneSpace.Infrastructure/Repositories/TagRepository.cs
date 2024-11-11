using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

public class TagRepository(TuneSpaceDbContext context) : ITagRepository
{
    public async Task InsertTag(Tag tag)
    {
        context.Tags.Add(tag);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Tag>> GetAllTags()
    {
        return await context.Tags.ToListAsync();
    }
}