namespace TuneSpace.Core.Entities;

public class RecommendationFeedback
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string BandId { get; set; } = string.Empty;
    public string BandName { get; set; } = string.Empty;
    public List<string> RecommendedGenres { get; set; } = [];
    public double InitialScore { get; set; } = 0.0;
    public DateTime RecommendedAt { get; set; } = DateTime.UtcNow;
    public FeedbackType FeedbackType { get; set; } = FeedbackType.None;
    public DateTime? FeedbackAt { get; set; }
    public double ExplicitRating { get; set; } = 0.0;
    public bool Clicked { get; set; } = false;
    public bool PlayedTrack { get; set; } = false;
    public bool FollowedBand { get; set; } = false;
    public bool SharedRecommendation { get; set; } = false;
    public bool SavedForLater { get; set; } = false;
    public TimeSpan? TimeSpentListening { get; set; }
    public Dictionary<string, double> ScoringFactors { get; set; } = [];
    public double CalculatedSuccess { get; set; } = 0.0;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}

public enum FeedbackType
{
    None,
    Positive,
    Negative,
    Mixed,
    Explicit
}
