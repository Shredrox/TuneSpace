using TuneSpace.Core.Models;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Provides vector-based search and recommendation services for music artists and bands.
/// This service utilizes vector embeddings to find similar artists and generate personalized recommendations.
/// </summary>
public interface IVectorSearchService
{
    /// <summary>
    /// Generates personalized artist recommendations for a user based on their listening history and preferences.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="topArtists">List of the user's top artists.</param>
    /// <param name="genres">List of genres the user is interested in.</param>
    /// <param name="recentlyPlayed">List of recently played artists by the user.</param>
    /// <param name="location">Optional location filter for local recommendations.</param>
    /// <param name="limit">Maximum number of recommendations to return. Default is 20.</param>
    /// <returns>A list of <see cref="BandModel"/> objects representing recommended artists.</returns>
    Task<List<BandModel>> RecommendArtistsForUserAsync(string userId, List<string> topArtists, List<string> genres, List<string> recentlyPlayed, string? location, int limit = 20);

    /// <summary>
    /// Generates contextual information for recommendations to help explain why certain artists were recommended.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="topArtists">List of the user's top artists.</param>
    /// <param name="genres">List of genres the user is interested in.</param>
    /// <param name="location">Optional location filter for context generation.</param>
    /// <returns>A string containing the recommendation context and reasoning.</returns>
    Task<string> GenerateRecommendationContextAsync(string userId, List<string> topArtists, List<string> genres, string? location);

    /// <summary>
    /// Indexes a single Bandcamp artist into the vector search system.
    /// </summary>
    /// <param name="artist">The <see cref="BandcampArtistModel"/> to index.</param>
    /// <returns>A task representing the asynchronous indexing operation.</returns>
    Task IndexBandcampArtistAsync(BandcampArtistModel artist);

    /// <summary>
    /// Indexes multiple Bandcamp artists into the vector search system in a single bulk operation.
    /// </summary>
    /// <param name="artists">A list of <see cref="BandcampArtistModel"/> objects to index.</param>
    /// <returns>A task representing the asynchronous bulk indexing operation.</returns>
    Task IndexBandcampArtistsBulkAsync(List<BandcampArtistModel> artists);

    /// <summary>
    /// Checks whether a specific artist is already indexed in the vector search system.
    /// </summary>
    /// <param name="artistName">The name of the artist to check.</param>
    /// <returns>True if the artist is indexed, false otherwise.</returns>
    Task<bool> IsArtistIndexedAsync(string artistName);

    /// <summary>
    /// Gets the total number of artists currently indexed in the vector search system.
    /// </summary>
    /// <returns>The count of indexed artists.</returns>
    Task<int> GetIndexedArtistCountAsync();
}
