using TuneSpace.Core.Models;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Service responsible for enriching band data with additional information from external sources.
/// </summary>
public interface IDataEnrichmentService
{
    /// <summary>
    /// Retrieves or creates cached band data for a specific band name.
    /// </summary>
    /// <param name="bandName">Name of the band to retrieve data for.</param>
    /// <returns>A band model with detailed information, or null if not found.</returns>
    Task<BandModel?> GetCachedBandDataAsync(string bandName);

    /// <summary>
    /// Finds bands similar to a list of artists, with options to filter and deduplicate results.
    /// </summary>
    /// <param name="artistNames">Names of artists to find similar bands for.</param>
    /// <param name="maxSimilarPerArtist">Maximum number of similar bands to return per artist. Default is 5.</param>
    /// <param name="processedBandNames">Set of band names that should be excluded from results. Default is null.</param>
    /// <param name="isRegisteredBandSimilar">Whether the similar bands are related to registered bands. Default is false.</param>
    /// <returns>A dictionary mapping artist names to lists of similar bands.</returns>
    Task<Dictionary<string, List<BandModel>>> GetSimilarBandsForMultipleArtistsAsync(
        List<string> artistNames,
        int maxSimilarPerArtist = 5,
        HashSet<string>? processedBandNames = null,
        bool isRegisteredBandSimilar = false
    );

    /// <summary>
    /// Enriches a list of band models with additional information like listener counts, play counts, and genres.
    /// </summary>
    /// <param name="bands">List of band models to enrich.</param>
    /// <returns>The enriched list of band models.</returns>
    Task<List<BandModel>> EnrichMultipleBandsAsync(List<BandModel> bands);
}
