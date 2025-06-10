using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IRepositories;

/// <summary>
/// Repository interface for managing recommendation feedback data operations
/// </summary>
public interface IRecommendationFeedbackRepository
{
    /// <summary>
    /// Retrieves a recommendation feedback by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the feedback</param>
    /// <returns>The feedback if found, null otherwise</returns>
    Task<RecommendationFeedback?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all feedback entries for a specific user
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <returns>A collection of feedback entries for the user</returns>
    Task<List<RecommendationFeedback>> GetByUserIdAsync(string userId);

    /// <summary>
    /// Retrieves feedback entries for a user within a specific time period
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <param name="daysPeriod">The number of days to look back</param>
    /// <returns>A collection of feedback entries within the time period</returns>
    Task<List<RecommendationFeedback>> GetUserFeedbackHistoryAsync(string userId, int daysPeriod = 30);

    /// <summary>
    /// Retrieves feedback entries for a specific band
    /// </summary>
    /// <param name="bandId">The unique identifier of the band</param>
    /// <returns>A collection of feedback entries for the band</returns>
    Task<List<RecommendationFeedback>> GetByBandIdAsync(string bandId);

    /// <summary>
    /// Retrieves feedback entries by user and band
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <param name="bandId">The unique identifier of the band</param>
    /// <returns>A collection of feedback entries for the user and band combination</returns>
    Task<List<RecommendationFeedback>> GetByUserAndBandAsync(string userId, string bandId);

    /// <summary>
    /// Gets the count of feedback entries by type for a user
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <param name="feedbackType">The type of feedback to count</param>
    /// <returns>The count of feedback entries of the specified type</returns>
    Task<int> GetCountByUserAndTypeAsync(string userId, FeedbackType feedbackType);

    /// <summary>
    /// Gets the total count of feedback entries for a user
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <returns>The total count of feedback entries</returns>
    Task<int> GetTotalCountByUserAsync(string userId);

    /// <summary>
    /// Creates a new recommendation feedback entry
    /// </summary>
    /// <param name="feedback">The feedback entity to create</param>
    /// <returns>The created feedback entity with generated values</returns>
    Task<RecommendationFeedback> InsertAsync(RecommendationFeedback feedback);

    /// <summary>
    /// Updates an existing recommendation feedback entry
    /// </summary>
    /// <param name="feedback">The feedback entity with updated values</param>
    Task UpdateAsync(RecommendationFeedback feedback);

    /// <summary>
    /// Deletes a recommendation feedback entry
    /// </summary>
    /// <param name="id">The unique identifier of the feedback to delete</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Gets engagement metrics aggregated for a user
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <returns>A dictionary containing engagement metric names and values</returns>
    Task<Dictionary<string, double>> GetUserEngagementMetricsAsync(string userId);

    /// <summary>
    /// Calculates the overall recommendation success rate for a user
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <returns>The success rate as a percentage (0.0 to 1.0)</returns>
    Task<double> GetUserSuccessRateAsync(string userId);
}
