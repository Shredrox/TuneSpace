using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IRepositories;

public interface IBandRepository
{
    Task InsertBand(Band band);
    Task<Band?> GetBandById(Guid id);
    Task<Band?> GetBandByName(string name);
    Task<Band?> GetBandByUserId(string id);
    Task UpdateBand(Band band);
    Task DeleteBand(Guid id);
}
