using TuneSpace.Core.Models;

namespace TuneSpace.Core.Interfaces.IClients;

public interface ILastFmClient
{
    Task<BandModel> GetBandDataAsync(string bandName);
    Task<List<string>> GetSimilarBandsAsync(string bandName, int limit);
    Task<List<BandModel>> EnrichBandData(List<BandModel> bands);
}
