using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

internal class MerchandiseRepository(TuneSpaceDbContext context) : IMerchandiseRepository
{
    private readonly TuneSpaceDbContext _context = context;

    async Task<Merchandise?> IMerchandiseRepository.GetMerchandiseByIdAsync(Guid merchandiseId)
    {
        return await _context.Merchandises
            .FirstOrDefaultAsync(m => m.Id == merchandiseId);
    }

    async Task<IEnumerable<Merchandise>> IMerchandiseRepository.GetAllMerchandisesAsync()
    {
        return await _context.Merchandises.ToListAsync();
    }

    async Task<IEnumerable<Merchandise>> IMerchandiseRepository.GetAllMerchandisesByBandIdAsync(Guid bandId)
    {
        return await _context.Merchandises
            .Where(m => m.BandId == bandId)
            .ToListAsync();
    }

    async Task<Merchandise> IMerchandiseRepository.InsertMerchandiseAsync(Merchandise merchandise)
    {
        _context.Merchandises.Add(merchandise);
        await _context.SaveChangesAsync();
        return merchandise;
    }

    async Task<Merchandise> IMerchandiseRepository.UpdateMerchandiseAsync(Merchandise merchandise)
    {
        _context.Merchandises.Update(merchandise);
        await _context.SaveChangesAsync();
        return merchandise;
    }

    async Task<bool> IMerchandiseRepository.DeleteMerchandiseAsync(Guid merchandiseId)
    {
        var merchandise = await _context.Merchandises.FindAsync(merchandiseId);
        if (merchandise is null)
        {
            return false;
        }

        _context.Merchandises.Remove(merchandise);
        await _context.SaveChangesAsync();
        return true;
    }
}
