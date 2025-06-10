using TuneSpace.Core.Models;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Enhanced AI prompt engineering service with sophisticated contextual prompts
/// </summary>
public interface IEnhancedAIPromptService
{
    /// <summary>
    /// Build an advanced context-aware recommendation prompt with user behavior analysis
    /// </summary>
    /// <param name="topArtists">List of user's top artists</param>
    /// <param name="genres">List of user's preferred genres</param>
    /// <param name="location">User's geographic location</param>
    /// <param name="vectorResults">Vector search results for similar artists</param>
    /// <param name="ragContext">Contextual music knowledge from RAG system</param>
    /// <param name="userBehaviorContext">Dictionary containing user behavior patterns and preferences</param>
    /// <returns>Formatted prompt string for AI recommendation generation</returns>
    string BuildAdvancedContextAwarePrompt(
        List<string> topArtists,
        List<string> genres,
        string location,
        List<BandModel> vectorResults,
        string ragContext,
        Dictionary<string, object> userBehaviorContext);

    /// <summary>
    /// Build an adaptive ranking prompt that considers user's current musical journey stage
    /// </summary>
    /// <param name="candidates">List of candidate artists to rank</param>
    /// <param name="topArtists">List of user's top artists</param>
    /// <param name="genres">List of user's preferred genres</param>
    /// <param name="location">User's geographic location</param>
    /// <param name="userJourneyStage">Current stage of user's musical discovery journey (discovery, deepening, comfort, exploration)</param>
    /// <param name="genreWeights">Optional dictionary of genre preference weights</param>
    /// <returns>Formatted prompt string for AI-powered artist ranking</returns>
    string BuildAdaptiveRankingPrompt(
        List<BandModel> candidates,
        List<string> topArtists,
        List<string> genres,
        string location,
        string userJourneyStage = "exploration",
        Dictionary<string, double>? genreWeights = null);

    /// <summary>
    /// Generate explanation prompts with enhanced contextual understanding
    /// </summary>
    /// <param name="band">The band/artist being recommended</param>
    /// <param name="userGenres">User's preferred genres</param>
    /// <param name="userTopArtists">User's top artists</param>
    /// <param name="recommendationContext">Additional context about why this recommendation was made</param>
    /// <returns>Formatted prompt string for generating personalized recommendation explanations</returns>
    string BuildEnhancedExplanationPrompt(
        BandModel band,
        List<string> userGenres,
        List<string> userTopArtists,
        string recommendationContext);

    /// <summary>
    /// Build an enhanced prompt for RAG-based music recommendations
    /// </summary>
    /// <param name="genres">List of user's preferred genres</param>
    /// <param name="location">User's geographic location</param>
    /// <param name="vectorResults">Vector search results for similar artists</param>
    /// <param name="ragContext">Contextual music knowledge from RAG system</param>
    /// <returns>Formatted prompt string for AI recommendation generation</returns>
    string BuildEnhancedPrompt(
        List<string> genres,
        string location,
        List<BandModel> vectorResults,
        string ragContext);

    /// <summary>
    /// Build an advanced recommendation prompt for analyzing vector search results and generating complementary artist suggestions
    /// </summary>
    /// <param name="topArtists">List of user's top artists</param>
    /// <param name="genres">List of user's preferred genres</param>
    /// <param name="location">User's geographic location</param>
    /// <param name="vectorResults">Vector search results for similar artists</param>
    /// <param name="ragContext">Contextual music knowledge from RAG system</param>
    /// <returns>Formatted prompt string for AI recommendation generation</returns>
    string BuildAdvancedRecommendationPrompt(
        List<string> topArtists,
        List<string> genres,
        string location,
        List<BandModel> vectorResults,
        string ragContext);

    /// <summary>
    /// Build a ranking prompt for AI-powered artist candidate evaluation and ordering
    /// </summary>
    /// <param name="candidates">List of candidate artists to rank</param>
    /// <param name="topArtists">List of user's top artists</param>
    /// <param name="genres">List of user's preferred genres</param>
    /// <param name="location">User's geographic location</param>
    /// <param name="limit">Maximum number of artists to rank</param>
    /// <returns>Formatted prompt string for AI-powered artist ranking</returns>
    string BuildRankingPrompt(
        List<BandModel> candidates,
        List<string> topArtists,
        List<string> genres,
        string location,
        int limit);
}
