using TuneSpace.Core.Models;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Provides services for discovering and recommending music artists based on user preferences and location.
/// </summary>
public interface IMusicDiscoveryService
{
    /// <summary>
    /// Generates band recommendations based on the user's music preferences and location using basic RAG AI.
    /// </summary>
    /// <param name="spotifyAccessToken">The Spotify access token.</param>
    /// <param name="genres">A list of music genres that the user is interested in.</param>
    /// <param name="location">The geographical location to find local or relevant bands.</param>
    /// <param name="userId">The unique identifier of the user for whom recommendations are being generated.</param>
    /// <returns>A list of recommended band models sorted by relevance to the user's preferences.</returns>
    Task<List<BandModel>> GetBandRecommendationsAsync(string? spotifyAccessToken, string userId, List<string> genres, string location);

    /// <summary>
    /// Generates enhanced band recommendations with confidence scoring and adaptive learning.
    /// </summary>
    /// <param name="spotifyAccessToken">The Spotify access token.</param>
    /// <param name="genres">A list of music genres that the user is interested in.</param>
    /// <param name="location">The geographical location to find local or relevant bands.</param>
    /// <param name="userId">The unique identifier of the user for whom recommendations are being generated.</param>
    /// <returns>A list of enhanced recommended band models with confidence scoring.</returns>
    Task<List<BandModel>> GetEnhancedBandRecommendationsAsync(string? spotifyAccessToken, string userId, List<string> genres, string location);

    /// <summary>
    /// Tracks user interaction with recommendations for adaptive learning feedback.
    /// </summary>
    /// <param name="userId">The unique identifier of the user providing feedback.</param>
    /// <param name="artistName">The name of the artist/band being rated.</param>
    /// <param name="interactionType">The type of interaction (like, dislike, thumbs_up, etc.).</param>
    /// <param name="genres">Optional list of music genres related to the interaction.</param>
    /// <param name="rating">Optional explicit rating score (0.0 to 5.0).</param>
    Task TrackRecommendationInteractionAsync(string userId, string artistName, string interactionType, List<string> genres, double rating = 0.0);

    /// <summary>
    /// Batch tracks multiple user interactions efficiently for adaptive learning.
    /// </summary>
    /// <param name="userId">The unique identifier of the user providing feedback.</param>
    /// <param name="interactions">List of interactions containing artist name, interaction type, and rating.</param>
    Task TrackBatchRecommendationInteractionsAsync(string userId, List<(string artistName, string interactionType, double rating, List<string> genres)> interactions);
}
