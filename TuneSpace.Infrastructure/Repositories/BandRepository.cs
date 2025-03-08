using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories
{
    internal class BandRepository(TuneSpaceDbContext context) : IBandRepository
    {
        async Task IBandRepository.InsertBand(Band band)
        {
            context.Bands.Add(band);
            await context.SaveChangesAsync();
        }

        async Task<Band?> IBandRepository.GetBandById(Guid id)
        {
            return await context.Bands.FindAsync(id);
        }

        async Task<Band?> IBandRepository.GetBandByName(string name)
        {
            return await context.Bands.FirstOrDefaultAsync(b => b.Name == name);
        }
    }
}
