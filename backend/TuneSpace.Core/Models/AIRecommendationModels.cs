namespace TuneSpace.Core.Models;

/// <summary>
/// AI recommendation with confidence scoring and reasoning
/// </summary>
public class AIRecommendationWithConfidence
{
    public string ArtistName { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public string Reasoning { get; set; } = string.Empty;
    public List<string> AlternativeNames { get; set; } = [];
    public List<string> MatchingGenres { get; set; } = [];
    public string ExternalUrl { get; set; } = string.Empty; // Link to artist profile
    public string RecommendationSource { get; set; } = "AI"; // AI, Vector, Hybrid
    public Dictionary<string, double> FeatureScores { get; set; } = [];
}

/// <summary>
/// Enhanced AI recommendation result with multiple candidates and confidence metrics
/// </summary>
public class EnhancedAIRecommendationResult
{
    public List<AIRecommendationWithConfidence> Recommendations { get; set; } = [];
    public double OverallConfidence { get; set; }
    public string PromptUsed { get; set; } = string.Empty;
    public Dictionary<string, object> ContextData { get; set; } = [];
    public TimeSpan ProcessingTime { get; set; }
    public bool UsedFallback { get; set; } = false;
}

/// <summary>
/// User's current musical journey context for AI prompts
/// </summary>
public class MusicalJourneyContext
{
    public string JourneyStage { get; set; } = "exploration"; // discovery, deepening, comfort, exploration
    public double ExplorationFactor { get; set; } = 0.5; // 0.0 = comfort zone, 1.0 = maximum exploration
    public List<string> EmergingGenres { get; set; } = []; // Genres user is starting to explore
    public List<string> MaturingGenres { get; set; } = []; // Genres user has deep experience with
    public Dictionary<string, double> GenreWeights { get; set; } = []; // Current preference weights
    public List<string> RecentSuccessfulRecommendations { get; set; } = []; // Recently liked artists
    public int DiscoveryReadinessScore { get; set; } = 50; // 0-100, how ready for new discoveries
}
