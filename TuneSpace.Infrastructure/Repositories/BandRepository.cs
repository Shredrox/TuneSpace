using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

internal class BandRepository(TuneSpaceDbContext context) : IBandRepository
{
    async Task IBandRepository.InsertBand(Band band)
    {
        context.Bands.Add(band);
        await context.SaveChangesAsync();
    }

    async Task<Band?> IBandRepository.GetBandByName(string name)
    {
        return await context.Bands.FirstOrDefaultAsync(b => b.Name == name);
    }

    async Task<Band?> IBandRepository.GetBandById(Guid id)
    {
        return await context.Bands.FindAsync(id);
    }

    async Task<Band?> IBandRepository.GetBandByUserId(string id)
    {
        return await context.Bands.FirstOrDefaultAsync((band) => band.UserId == id);
    }

    async Task IBandRepository.UpdateBand(Band band)
    {
        context.Bands.Update(band);
        await context.SaveChangesAsync();
    }

    async Task IBandRepository.DeleteBand(Guid id)
    {
        var band = await context.Bands.FindAsync(id) ?? throw new KeyNotFoundException($"Band with ID {id} not found.");
        context.Bands.Remove(band);
        await context.SaveChangesAsync();
    }
}
