using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

internal class BandRepository(TuneSpaceDbContext context) : IBandRepository
{
    private readonly TuneSpaceDbContext _context = context;

    async Task<List<Band>> IBandRepository.GetAllBandsAsync()
    {
        return await _context.Bands.ToListAsync();
    }

    async Task<List<Band>> IBandRepository.GetBandsByGenreAsync(string genre)
    {
        return await _context.Bands
            .Where(b => b.Genre != null && b.Genre.Contains(genre))
            .ToListAsync();
    }

    async Task<List<Band>> IBandRepository.GetBandsByLocationAsync(string location)
    {
        return await _context.Bands
            .Where(b => (b.Country != null && b.Country.Contains(location)) ||
                        (b.City != null && b.City.Contains(location)))
            .ToListAsync();
    }

    async Task<List<Band>> IBandRepository.GetBandsByGenreAndLocationAsync(string genre, string location)
    {
        return await _context.Bands
            .Where(b =>
                b.Genre != null && b.Genre.Contains(genre) &&
                ((b.Country != null && b.Country.Contains(location)) ||
                 (b.City != null && b.City.Contains(location))))
            .ToListAsync();
    }

    async Task<Band?> IBandRepository.GetBandByNameAsync(string name)
    {
        return await _context.Bands.FirstOrDefaultAsync(b => b.Name == name);
    }

    async Task<Band?> IBandRepository.GetBandByIdAsync(Guid id)
    {
        return await _context.Bands.Include(b => b.Members).FirstOrDefaultAsync(b => b.Id == id);
    }

    async Task<Band?> IBandRepository.GetBandByUserIdAsync(string id)
    {
        return await _context.Bands
            .Include(b => b.Members)
            .FirstOrDefaultAsync((band) => band.Members
                .Select(m => m.Id)
                .Contains(Guid.Parse(id)));
    }

    async Task IBandRepository.InsertBandAsync(Band band)
    {
        _context.Bands.Add(band);
        await _context.SaveChangesAsync();
    }

    async Task IBandRepository.UpdateBandAsync(Band band)
    {
        _context.Bands.Update(band);
        await _context.SaveChangesAsync();
    }

    async Task IBandRepository.DeleteBandAsync(Guid id)
    {
        var band = await _context.Bands.FindAsync(id) ?? throw new KeyNotFoundException($"Band with ID {id} not found.");
        _context.Bands.Remove(band);
        await _context.SaveChangesAsync();
    }
}
