using TuneSpace.Core.DTOs.Requests.Band;
using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IServices
{
    public interface IBandService
    {
        Task<Band?> CreateBand(CreateBandRequest request);
        Task<Band?> GetBandById(Guid id);
        Task<Band?> GetBandByName(string name);
        Task<Band?> GetBandByUserId(string id);
        Task<byte[]?> GetBandImage(Guid bandId);
        Task UpdateBand(UpdateBandRequest request);
        Task DeleteBand(Guid id);
    }
}
