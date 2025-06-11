using TuneSpace.Core.Models;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Service for collaborative filtering recommendations based on user similarity
/// </summary>
public interface ICollaborativeFilteringService
{    /// <summary>
     /// Calculate similarity between two users based on their listening behavior
     /// </summary>
    UserSimilarity CalculateUserSimilarity(string userId1, string userId2);

    /// <summary>
    /// Find users similar to the target user
    /// </summary>
    Task<List<UserSimilarity>> FindSimilarUsersAsync(string userId, int maxResults = 10, double minSimilarity = 0.3);

    /// <summary>
    /// Get collaborative filtering recommendations for a user
    /// </summary>
    Task<List<CollaborativeRecommendation>> GetCollaborativeRecommendationsAsync(string userId, int maxRecommendations = 20);

    /// <summary>
    /// Update user listening behavior for collaborative filtering
    /// </summary>
    Task UpdateUserListeningBehaviorAsync(string userId, string spotifyAccessToken);

    /// <summary>
    /// Get users with similar taste profiles
    /// </summary>
    Task<List<string>> GetUserCohortAsync(string userId, int cohortSize = 50);
    /// <summary>
    /// Calculate genre-based similarity between users
    /// </summary>
    double CalculateGenreSimilarity(string userId1, string userId2);

    /// <summary>
    /// Calculate artist-based similarity between users
    /// </summary>
    double CalculateArtistSimilarity(string userId1, string userId2);

    /// <summary>
    /// Get the number of cached similarity calculations
    /// </summary>
    int GetCachedSimilarityCount();

    /// <summary>
    /// Clear the similarity cache (useful for testing or memory management)
    /// </summary>
    void ClearSimilarityCache();
}
