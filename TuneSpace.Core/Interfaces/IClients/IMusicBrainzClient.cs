using TuneSpace.Core.Models;

namespace TuneSpace.Core.Interfaces.IClients;

public interface IMusicBrainzClient
{
    Task<BandModel> GetBandDataAsync(string bandName);
    Task<List<BandModel>> GetBandsByLocationAsync(string location, int limit, List<string>? genres = null);
}
