using System.Collections.Concurrent;
using TuneSpace.Core.DTOs.Responses.Spotify;
using TuneSpace.Core.Models;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Service responsible for scoring bands based on user preferences and applying diversity algorithms.
/// </summary>
public interface IRecommendationScoringService
{
    /// <summary>
    /// Scores a list of bands based on various factors including genre match, location, popularity, and more.
    /// </summary>
    /// <param name="bands">List of band models to score.</param>
    /// <param name="userGenres">List of genres preferred by the user.</param>
    /// <param name="location">User's location for location-based scoring.</param>
    /// <param name="topArtists">List of user's top artists from Spotify.</param>
    /// <param name="isRegistered">Whether the bands being scored are registered in the system.</param>
    /// <param name="isFromSearch">Whether the bands are from a search result. Default is false.</param>
    /// <returns>List of scored band models with RelevanceScore property populated.</returns>
    List<BandModel> ScoreBands(
        List<BandModel> bands,
        List<string> userGenres,
        string location,
        List<SpotifyArtistDTO> topArtists,
        bool isRegistered,
        bool isFromSearch = false);

    /// <summary>
    /// Applies diversity and exploration algorithms to create a balanced set of recommendations.
    /// </summary>
    /// <param name="recommendedBands">Initial list of scored band recommendations.</param>
    /// <param name="previouslyRecommendedBands">Dictionary of previously recommended band IDs and when they were recommended.</param>
    /// <returns>A diverse list of recommended bands optimized for discovery.</returns>
    List<BandModel> ApplyDiversityAndExploration(List<BandModel> recommendedBands, ConcurrentDictionary<string, DateTime> previouslyRecommendedBands);
}
