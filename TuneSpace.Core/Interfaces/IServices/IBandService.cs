using TuneSpace.Core.DTOs.Requests.Band;
using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IServices
{
    public interface IBandService
    {
        Task CreateBand(CreateBandRequest request);
        Task<Band?> GetBandById(Guid id);
        Task<Band?> GetBandByName(string name);
        Task<byte[]?> GetBandImage(Guid bandId);
    }
}
