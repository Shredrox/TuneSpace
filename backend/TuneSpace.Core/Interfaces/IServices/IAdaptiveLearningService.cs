using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Service interface for adaptive learning functionality that personalizes music recommendations
/// based on user behavior, preferences, and feedback patterns.
/// </summary>
public interface IAdaptiveLearningService
{
    /// <summary>
    /// Retrieves the current dynamic scoring weights for a specific user.
    /// These weights determine how different factors (genre preference, popularity, etc.)
    /// are weighted in the recommendation algorithm.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task that represents the asynchronous operation containing the user's dynamic scoring weights.</returns>
    Task<DynamicScoringWeights> GetUserScoringWeightsAsync(string userId);

    /// <summary>
    /// Updates the scoring weights for a user based on their feedback to a recommendation.
    /// This method adapts the recommendation algorithm based on positive or negative feedback.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="feedback">The recommendation feedback containing the user's response.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateScoringWeightsAsync(string userId, RecommendationFeedback feedback);

    /// <summary>
    /// Automatically adapts scoring weights based on the user's historical success rate with recommendations.
    /// This method analyzes patterns in user behavior to optimize future recommendations.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AdaptWeightsBasedOnSuccessRateAsync(string userId);

    /// <summary>
    /// Retrieves the evolution of a user's genre preferences over time.
    /// This provides insights into how the user's musical taste has changed.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task that represents the asynchronous operation containing a list of genre evolution data.</returns>
    Task<List<GenreEvolution>> GetUserGenreEvolutionAsync(string userId);

    /// <summary>
    /// Updates a user's genre preferences based on their feedback to specific genres.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="genres">The list of genres that received feedback.</param>
    /// <param name="feedbackType">The type of feedback (positive, negative, neutral).</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateGenrePreferencesAsync(string userId, List<string> genres, FeedbackType feedbackType);

    /// <summary>
    /// Predicts how a user's genre preferences might evolve in the future based on historical patterns.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="daysAhead">The number of days into the future to predict (default: 30 days).</param>
    /// <returns>A task that represents the asynchronous operation containing predicted genre preferences with confidence scores.</returns>
    Task<Dictionary<string, double>> PredictFutureGenrePreferencesAsync(string userId, int daysAhead = 30);

    /// <summary>
    /// Processes recommendation feedback to update the adaptive learning models.
    /// This is the main entry point for incorporating user feedback into the learning system.
    /// </summary>
    /// <param name="feedback">The recommendation feedback from the user.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ProcessRecommendationFeedbackAsync(RecommendationFeedback feedback);

    /// <summary>
    /// Calculates the success rate of a recommendation based on user feedback.
    /// </summary>
    /// <param name="feedback">The recommendation feedback to evaluate.</param>
    /// <returns>A double value representing the success score (typically between 0.0 and 1.0).</returns>
    double CalculateRecommendationSuccess(RecommendationFeedback feedback);

    /// <summary>
    /// Triggers periodic adaptation of the learning model for a specific user.
    /// This method should be called regularly to ensure the recommendation system stays current.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task TriggerPeriodicAdaptationAsync(string userId);

    /// <summary>
    /// Analyzes genre trends for a specific user over a specified time period.
    /// This helps identify emerging patterns in the user's musical preferences.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="daysPeriod">The number of days to analyze for trends (default: 90 days).</param>
    /// <returns>A task that represents the asynchronous operation containing genre trends with trend scores.</returns>
    Task<Dictionary<string, double>> GetGenreTrendsAsync(string userId, int daysPeriod = 90);

    /// <summary>
    /// Determines the lifecycle stage of a specific genre for a user (emerging, peak, declining, dormant).
    /// This helps optimize when to recommend music from different genres.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="genre">The genre to analyze.</param>
    /// <returns>A task that represents the asynchronous operation containing the genre's lifecycle stage.</returns>
    Task<GenreLifecycleStage> DetermineGenreLifecycleStageAsync(string userId, string genre);

    /// <summary>
    /// Identifies emerging genres that might interest the user based on their preference patterns.
    /// This enables discovery of new musical styles aligned with the user's taste evolution.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task that represents the asynchronous operation containing a list of emerging genres for the user.</returns>
    Task<List<string>> GetEmergingGenresForUserAsync(string userId);

    /// <summary>
    /// Retrieves the current exploration factor for a user, which determines how willing
    /// the system should be to recommend unfamiliar content versus safe choices.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task that represents the asynchronous operation containing the exploration factor (typically between 0.0 and 1.0).</returns>
    Task<double> GetExplorationFactorAsync(string userId);

    /// <summary>
    /// Updates the exploration factor for a user based on whether their last exploratory recommendation was successful.
    /// A higher exploration factor encourages more diverse recommendations.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="wasSuccessful">Whether the exploratory recommendation was successful.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateExplorationFactorAsync(string userId, bool wasSuccessful);
}
