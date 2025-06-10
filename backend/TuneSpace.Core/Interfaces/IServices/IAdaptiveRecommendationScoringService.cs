using TuneSpace.Core.DTOs.Responses.Spotify;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Models;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Enhanced recommendation scoring service that uses dynamic adaptive weights
/// to provide personalized band recommendations based on user behavior and preferences.
/// </summary>
public interface IAdaptiveRecommendationScoringService : IRecommendationScoringService
{
    /// <summary>
    /// Scores a list of bands using adaptive weighting algorithms that learn from user interactions
    /// and preferences to provide personalized recommendations.
    /// </summary>
    /// <param name="bands">The list of bands to score for recommendation ranking.</param>
    /// <param name="userGenres">The user's preferred music genres for genre-based scoring.</param>
    /// <param name="userId">The unique identifier of the user for personalization.</param>
    /// <param name="location">The user's location for location-based scoring preferences.</param>
    /// <param name="topArtists">The user's top artists from Spotify for similarity scoring.</param>
    /// <param name="isRegistered">Whether the bands are registered in the system (affects scoring weights).</param>
    /// <param name="isFromSearch">Whether the bands come from search results (affects scoring algorithm).</param>
    /// <returns>A list of scored and ranked band models ordered by recommendation relevance.</returns>
    Task<List<BandModel>> ScoreBandsWithAdaptiveWeightsAsync(
        List<BandModel> bands,
        List<string> userGenres,
        string userId,
        string location,
        List<SpotifyArtistDTO> topArtists,
        bool isRegistered,
        bool isFromSearch);

    /// <summary>
    /// Calculates individual scoring factors for a specific band to understand
    /// how different criteria contribute to the overall recommendation score.
    /// </summary>
    /// <param name="band">The band to calculate scoring factors for.</param>
    /// <param name="userGenres">The user's preferred music genres.</param>
    /// <param name="location">The user's location for location-based scoring.</param>
    /// <param name="topArtists">The user's top artists from Spotify for similarity analysis.</param>
    /// <param name="isRegistered">Whether the band is registered in the system.</param>
    /// <param name="isFromSearch">Whether the band comes from search results.</param>
    /// <param name="weights">The dynamic scoring weights to apply to different factors.</param>
    /// <returns>A dictionary containing scoring factor names and their calculated values.</returns>
    Task<Dictionary<string, double>> GetScoringFactorsForBand(
        BandModel band,
        List<string> userGenres,
        string location,
        List<SpotifyArtistDTO> topArtists,
        bool isRegistered,
        bool isFromSearch,
        DynamicScoringWeights weights);

    /// <summary>
    /// Records user interactions with recommended bands to improve future recommendations
    /// through machine learning and adaptive weight adjustment.
    /// </summary>
    /// <param name="userId">The unique identifier of the user who interacted with the recommendation.</param>
    /// <param name="bandId">The unique identifier of the band that was recommended.</param>
    /// <param name="bandName">The name of the band for logging and analysis purposes.</param>
    /// <param name="genres">The genres associated with the recommended band.</param>
    /// <param name="scoringFactors">The detailed scoring factors that led to this recommendation.</param>
    /// <param name="initialScore">The initial recommendation score before user interaction.</param>
    /// <returns>A task representing the asynchronous recording operation.</returns>
    Task RecordRecommendationInteractionAsync(
        string userId,
        string bandId,
        string bandName,
        List<string> genres,
        Dictionary<string, double> scoringFactors,
        double initialScore);
}
