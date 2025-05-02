using TuneSpace.Core.Models;

namespace TuneSpace.Core.Interfaces.IClients;

/// <summary>
/// Provides methods for interacting with the Last.fm API to retrieve music artist/band information.
/// </summary>
public interface ILastFmClient
{
    /// <summary>
    /// Retrieves detailed information about a band by its name from Last.fm.
    /// </summary>
    /// <param name="bandName">The name of the band to search for.</param>
    /// <returns>A task that represents the asynchronous operation, containing band information in a <see cref="BandModel"/> object.</returns>
    Task<BandModel> GetBandDataAsync(string bandName);

    /// <summary>
    /// Fetches a list of similar bands for a given band name from Last.fm's similar artists data.
    /// </summary>
    /// <param name="bandName">The name of the band to find similar artists for.</param>
    /// <param name="limit">The maximum number of similar bands to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of similar band names.</returns>
    Task<List<string>> GetSimilarBandsAsync(string bandName, int limit);
}
