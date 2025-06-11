namespace TuneSpace.Core.Models;

/// <summary>
/// Represents similarity between two users for collaborative filtering
/// </summary>
public class UserSimilarity
{
    public string UserId1 { get; set; } = string.Empty;
    public string UserId2 { get; set; } = string.Empty;
    public double SimilarityScore { get; set; } = 0.0; // 0.0 to 1.0
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    public string SimilarityBasis { get; set; } = string.Empty; // "genres", "artists", "temporal", "combined"

    // Breakdown of similarity components
    public double GenreSimilarity { get; set; } = 0.0;
    public double ArtistSimilarity { get; set; } = 0.0;
    public double TemporalSimilarity { get; set; } = 0.0;

    // Common preferences
    public List<string> CommonGenres { get; set; } = [];
    public List<string> CommonArtists { get; set; } = [];
    public int InteractionCount { get; set; } = 0; // Number of common interactions
}

/// <summary>
/// User's listening behavior summary for collaborative filtering
/// </summary>
public class UserListeningBehavior
{
    public string UserId { get; set; } = string.Empty;
    public Dictionary<string, double> GenreWeights { get; set; } = []; // Genre -> Weight (0.0 to 1.0)
    public Dictionary<string, double> ArtistWeights { get; set; } = []; // Artist ID -> Weight
    public List<string> TopGenres { get; set; } = [];
    public List<string> TopArtists { get; set; } = [];
    public double DiscoveryScore { get; set; } = 0.5; // How much they like discovering new music
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    // Listening intensity and patterns
    public double WeeklyListeningHours { get; set; } = 0.0;
    public double AverageSessionLength { get; set; } = 30.0;
    public double SkipRate { get; set; } = 0.0;
    public bool IsActiveDiscoverer { get; set; } = false; // Actively seeks new music
}

/// <summary>
/// Collaborative filtering recommendation result
/// </summary>
public class CollaborativeRecommendation
{
    public string ArtistId { get; set; } = string.Empty;
    public string ArtistName { get; set; } = string.Empty;
    public double Score { get; set; } = 0.0;
    public List<string> RecommendedBySimilarUsers { get; set; } = []; // User IDs
    public double AverageUserRating { get; set; } = 0.0;
    public string ReasoningExplanation { get; set; } = string.Empty;
    public List<string> CommonGenres { get; set; } = [];
}
