using TuneSpace.Core.Models;

namespace TuneSpace.Core.Interfaces.IClients;

/// <summary>
/// Provides methods for interacting with the MusicBrainz API to retrieve music artist/band information.
/// </summary>
public interface IMusicBrainzClient
{
    /// <summary>
    /// Retrieves detailed information about a band by its name.
    /// </summary>
    /// <param name="bandName">The name of the band to search for.</param>
    /// <returns>A task that represents the asynchronous operation, containing band information in a <see cref="BandModel"/> object.</returns>
    Task<BandModel> GetBandDataAsync(string bandName);

    /// <summary>
    /// Searches for bands based on geographical location and optional genre filters.
    /// </summary>
    /// <param name="location">The geographical location (city, country) to search bands from.</param>
    /// <param name="limit">The maximum number of results to retrieve.</param>
    /// <param name="genres">Optional list of genres to filter the results by. If null, no genre filtering is applied.</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of bands matching the search criteria.</returns>
    Task<List<BandModel>> GetBandsByLocationAsync(string location, int limit, List<string>? genres);
}
