using TuneSpace.Core.Models;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Service interface for AI-powered music recommendations using machine learning and retrieval-augmented generation (RAG).
/// Provides enhanced music discovery capabilities by combining user preferences, Spotify data, and AI insights.
/// </summary>
public interface IAIRecommendationService
{
    /// <summary>
    /// Gets AI-enhanced band recommendations using Retrieval-Augmented Generation (RAG) technology.
    /// Combines user's music preferences with contextual data to provide personalized band suggestions.
    /// </summary>
    /// <param name="spotifyAccessToken">Valid Spotify API access token for retrieving user's music data</param>
    /// <param name="userId">Unique identifier of the user requesting recommendations</param>
    /// <param name="topArtists">List of the user's top artists to base recommendations on</param>
    /// <param name="genres">List of preferred music genres to filter recommendations</param>
    /// <param name="location">User's location for geographically relevant recommendations</param>
    /// <param name="limit">Maximum number of band recommendations to return (default: 20)</param>
    /// <returns>A list of recommended bands tailored to the user's preferences and context</returns>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null or empty</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when Spotify access token is invalid</exception>
    Task<List<BandModel>> GetRAGEnhancedRecommendationsAsync(
        string spotifyAccessToken,
        string userId,
        List<string> topArtists,
        List<string> genres,
        string location,
        int limit = 20);

    /// <summary>
    /// Gets enhanced AI recommendations with confidence scoring and detailed analysis.
    /// Provides not only band suggestions but also confidence metrics and reasoning behind each recommendation.
    /// </summary>
    /// <param name="spotifyAccessToken">Valid Spotify API access token for retrieving user's music data</param>
    /// <param name="userId">Unique identifier of the user requesting recommendations</param>
    /// <param name="topArtists">List of the user's top artists to base recommendations on</param>
    /// <param name="genres">List of preferred music genres to filter recommendations</param>
    /// <param name="location">User's location for geographically relevant recommendations</param>
    /// <param name="limit">Maximum number of band recommendations to return (default: 20)</param>
    /// <returns>Enhanced recommendation result containing bands with confidence scores and analysis</returns>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null or empty</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when Spotify access token is invalid</exception>
    Task<EnhancedAIRecommendationResult> GetEnhancedRecommendationsWithConfidenceAsync(
        string spotifyAccessToken,
        string userId,
        List<string> topArtists,
        List<string> genres,
        string location,
        int limit = 20);

    /// <summary>
    /// Builds a comprehensive musical journey context for a user by analyzing their listening history,
    /// preferences, and behavioral patterns. This context is used to enhance future recommendations
    /// and provide more personalized music discovery experiences.
    /// </summary>
    /// <param name="userId">Unique identifier of the user to build the context for</param>
    /// <returns>A musical journey context containing user's music profile, preferences, and patterns</returns>
    /// <exception cref="ArgumentNullException">Thrown when userId is null or empty</exception>
    /// <exception cref="InvalidOperationException">Thrown when user data is insufficient to build context</exception>
    Task<MusicalJourneyContext> BuildMusicalJourneyContextAsync(string userId);
}
