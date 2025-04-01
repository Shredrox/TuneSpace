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

    async Task<List<Band>> IBandRepository.GetAllBands()
    {
        return await context.Bands.ToListAsync();
    }

    async Task<List<Band>> IBandRepository.GetBandsByGenre(string genre)
    {
        return await context.Bands
            .Where(b => b.Genre != null && b.Genre.Contains(genre))
            .ToListAsync();
    }

    async Task<List<Band>> IBandRepository.GetBandsByLocation(string location)
    {
        return await context.Bands
            .Where(b => (b.Country != null && b.Country.Contains(location)) || 
                        (b.City != null && b.City.Contains(location)))
            .ToListAsync();
    }

    async Task<List<Band>> IBandRepository.GetBandsByGenreAndLocation(string genre, string location)
    {
        return await context.Bands
            .Where(b => 
                b.Genre != null && b.Genre.Contains(genre) && 
                ((b.Country != null && b.Country.Contains(location)) || 
                 (b.City != null && b.City.Contains(location))))
            .ToListAsync();
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
