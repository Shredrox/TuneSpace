using Pgvector;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Service for generating and managing vector embeddings used in music recommendation algorithms.
/// Provides functionality to create embeddings for text, artists, and user preferences,
/// and calculate similarity scores between different entities.
/// </summary>
public interface IEmbeddingService
{
    /// <summary>
    /// Generates a vector embedding for a given text input using machine learning models.
    /// </summary>
    /// <param name="text">The input text to generate an embedding for.</param>
    /// <returns>A vector representation of the input text for similarity calculations.</returns>
    Task<Vector> GenerateEmbeddingAsync(string text);

    /// <summary>
    /// Generates vector embeddings for multiple text inputs in a single batch operation
    /// for improved performance when processing multiple items.
    /// </summary>
    /// <param name="texts">A list of text inputs to generate embeddings for.</param>
    /// <returns>A list of vector embeddings corresponding to each input text.</returns>
    Task<List<Vector>> GenerateBatchEmbeddingsAsync(List<string> texts);

    /// <summary>
    /// Generates a specialized vector embedding for an artist/band that incorporates
    /// multiple attributes to create a comprehensive representation for recommendation matching.
    /// </summary>
    /// <param name="artistName">The name of the artist or band.</param>
    /// <param name="genres">The musical genres associated with the artist.</param>
    /// <param name="location">The geographic location of the artist (optional).</param>
    /// <param name="description">Additional description or bio information about the artist (optional).</param>
    /// <returns>A vector embedding representing the artist's musical and contextual characteristics.</returns>
    Task<Vector> GenerateArtistEmbeddingAsync(string artistName, List<string> genres, string? location, string? description);

    /// <summary>
    /// Generates a personalized vector embedding representing a user's music preferences
    /// based on their listening history and preferred genres.
    /// </summary>
    /// <param name="topArtists">The user's most listened-to artists.</param>
    /// <param name="genres">The user's preferred music genres.</param>
    /// <param name="recentlyPlayed">The user's recently played tracks or artists.</param>
    /// <returns>A vector embedding representing the user's music taste profile.</returns>
    Task<Vector> GenerateUserPreferenceEmbeddingAsync(List<string> topArtists, List<string> genres, List<string> recentlyPlayed);

    /// <summary>
    /// Calculates the similarity score between two vector embeddings using cosine similarity
    /// or other distance metrics to determine how closely related two entities are.
    /// </summary>
    /// <param name="embedding1">The first vector embedding to compare.</param>
    /// <param name="embedding2">The second vector embedding to compare.</param>
    /// <returns>A similarity score between 0 and 1, where 1 indicates identical vectors.</returns>
    double CalculateSimilarity(Vector embedding1, Vector embedding2);

    /// <summary>
    /// Checks if the embedding service is available and functioning properly.
    /// Useful for health checks and ensuring the ML models are accessible.
    /// </summary>
    /// <returns>True if the service is available and ready to generate embeddings, false otherwise.</returns>
    Task<bool> IsServiceAvailableAsync();
}
