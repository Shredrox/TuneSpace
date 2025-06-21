using TuneSpace.Core.DTOs.Responses.Spotify;
using TuneSpace.Core.Models;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Service responsible for discovering and retrieving artist/band information from various sources.
/// </summary>
public interface IArtistDiscoveryService
{
    /// <summary>
    /// Retrieves detailed information about multiple artists in batches to avoid API rate limits.
    /// </summary>
    /// <param name="token">Spotify access token.</param>
    /// <param name="artistIds">List of Spotify artist IDs to retrieve.</param>
    /// <param name="batchSize">Maximum number of artists to request in a single API call. Default is 50.</param>
    /// <returns>A list of artist details from Spotify.</returns>
    Task<List<SpotifyArtistDTO>?> GetArtistDetailsInBatchesAsync(string token, List<string> artistIds, int batchSize = 50);

    /// <summary>
    /// Searches for artists matching specific genre criteria using a query template.
    /// </summary>
    /// <param name="token">Spotify access token.</param>
    /// <param name="genres">List of genres to search within.</param>
    /// <param name="queryTemplate">Template string for building the search query, with {genre} as placeholder.</param>
    /// <param name="limit">Maximum number of artists to return.</param>
    /// <returns>A list of band models matching the search criteria.</returns>
    Task<List<BandModel>> FindArtistsByQueryAsync(string token, List<string> genres, string queryTemplate, int limit);

    /// <summary>
    /// Discovers hipster and emerging new artists using specialized search criteria.
    /// This method focuses on finding lesser-known artists and bands that are gaining popularity.
    /// </summary>
    /// <param name="token">Spotify access token for API authentication.</param>
    /// <param name="genres">List of genres to search within for targeted discovery.</param>
    /// <param name="limit">Maximum number of artists to return in the search results.</param>
    /// <returns>A list of band models representing hipster and newly discovered artists.</returns>
    Task<List<BandModel>> FindHipsterAndNewArtistsAsync(string token, List<string> genres, int limit);

    /// <summary>
    /// Retrieves bands registered in the system as BandModel objects, enriched with additional information.
    /// </summary>
    /// <param name="genres">List of genres to filter by.</param>
    /// <param name="location">Location (country or city) to filter by.</param>
    /// <returns>A list of band models for registered bands matching the criteria.</returns>
    Task<List<BandModel>> GetRegisteredBandsAsModelsAsync(List<string> genres, string location);
}
