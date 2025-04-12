using TuneSpace.Core.Models;

namespace TuneSpace.Core.Interfaces.IServices;

public interface IMusicDiscoveryService
{
    Task<List<BandModel>> GetBandRecommendationsAsync(string spotifyAccessToken, List<string> genres, string location);
}
