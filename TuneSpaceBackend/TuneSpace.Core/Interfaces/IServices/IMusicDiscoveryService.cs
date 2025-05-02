using TuneSpace.Core.Models;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Provides services for discovering and recommending music artists based on user preferences and location.
/// </summary>
public interface IMusicDiscoveryService
{
    /// <summary>
    /// Generates band recommendations based on the user's music preferences and location.
    /// </summary>
    /// <param name="spotifyAccessToken">The Spotify access token used to retrieve user preferences and music data.</param>
    /// <param name="genres">A list of music genres that the user is interested in.</param>
    /// <param name="location">The geographical location to find local or relevant bands.</param>
    /// <returns>A list of recommended band models sorted by relevance to the user's preferences.</returns>
    Task<List<BandModel>> GetBandRecommendationsAsync(string spotifyAccessToken, List<string> genres, string location);
}
