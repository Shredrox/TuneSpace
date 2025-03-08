using TuneSpace.Core.DTOs.Requests.Band;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Core.Services
{
    internal class BandService(IBandRepository bandRepository) : IBandService
    {
        async Task IBandService.CreateBand(CreateBandRequest request)
        {
            var city = request.Location.Split(',')[1].Trim();
            var country = request.Location.Split(',')[0].Trim();

            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await request.Picture.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            await bandRepository.InsertBand(new Band
            {
                Name = request.Name,
                Genre = request.Genre,
                Description = request.Description,
                Country = country,
                City = city,
                CoverImage = fileBytes
            });
        }

        async Task<Band?> IBandService.GetBandById(Guid id)
        {
            return await bandRepository.GetBandById(id);
        }

        async Task<Band?> IBandService.GetBandByName(string name)
        {
            return await bandRepository.GetBandByName(name);
        }

        async Task<byte[]?> IBandService.GetBandImage(Guid bandId)
        {
            var band = await bandRepository.GetBandById(bandId);
            return band?.CoverImage;
        }
    }
}
