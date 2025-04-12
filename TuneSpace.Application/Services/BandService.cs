using TuneSpace.Core.DTOs.Requests.Band;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Application.Services;

internal class BandService(IBandRepository bandRepository, IUserRepository userRepository) : IBandService
{
    async Task<Band?> IBandService.CreateBand(CreateBandRequest request)
    {
        var user = await userRepository.GetUserById(request.UserId);
        if (user == null)
        {
            return null;
        }

        var city = request.Location.Split(',')[1].Trim();
        var country = request.Location.Split(',')[0].Trim();

        byte[] fileBytes;
        using (var memoryStream = new MemoryStream())
        {
            await request.Picture.CopyToAsync(memoryStream);
            fileBytes = memoryStream.ToArray();
        }

        var band = new Band
        {
            Name = request.Name,
            Genre = request.Genre,
            Description = request.Description,
            Country = country,
            City = city,
            CoverImage = fileBytes,
            User = user
        };

        await bandRepository.InsertBand(band);
        return band;
    }

    async Task<Band?> IBandService.GetBandById(Guid id)
    {
        return await bandRepository.GetBandById(id);
    }

    async Task<Band?> IBandService.GetBandByName(string name)
    {
        return await bandRepository.GetBandByName(name);
    }

    async Task<Band?> IBandService.GetBandByUserId(string id)
    {
        return await bandRepository.GetBandByUserId(id);
    }

    async Task<byte[]?> IBandService.GetBandImage(Guid bandId)
    {
        var band = await bandRepository.GetBandById(bandId);
        return band?.CoverImage;
    }

    async Task IBandService.UpdateBand(UpdateBandRequest request)
    {
        var band = await bandRepository.GetBandById(request.Id);
        if (band == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            band.Name = request.Name;
        }

        if (!string.IsNullOrEmpty(request.Description))
        {
            band.Description = request.Description;
        }

        if (!string.IsNullOrEmpty(request.Genre))
        {
            band.Genre = request.Genre;
        }

        if (!string.IsNullOrEmpty(request.SpotifyId))
        {
            band.SpotifyId = request.SpotifyId;
        }

        if (!string.IsNullOrEmpty(request.YouTubeEmbedId))
        {
            band.YouTubeEmbedId = request.YouTubeEmbedId;
        }

        if (request.Picture != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await request.Picture.CopyToAsync(memoryStream);
                band.CoverImage = memoryStream.ToArray();
            }
        }

        await bandRepository.UpdateBand(band);
    }

    async Task IBandService.DeleteBand(Guid id)
    {
        try
        {
            await bandRepository.DeleteBand(id);
        }
        catch (KeyNotFoundException)
        {

        }
    }
}
