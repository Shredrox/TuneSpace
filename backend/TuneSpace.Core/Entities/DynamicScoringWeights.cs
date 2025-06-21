namespace TuneSpace.Core.Entities;

public class DynamicScoringWeights
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public double GenreMatchWeight { get; set; } = 0.3;
    public double LocationMatchWeight { get; set; } = 0.3;
    public double ListenerScoreWeight { get; set; } = 0.2;
    public double SimilarArtistWeight { get; set; } = 0.2;
    public double UndergroundBandWeight { get; set; } = 0.25;
    public double NewReleaseWeight { get; set; } = 0.2;
    public double RegisteredBandWeight { get; set; } = 0.5;
    public double ExplorationFactor { get; set; } = 0.1;
    public double DiversityFactor { get; set; } = 0.15;
    public double LearningRate { get; set; } = 0.01;
    public int RecommendationCount { get; set; } = 0;
    public int PositiveFeedbackCount { get; set; } = 0;
    public double SuccessRate { get; set; } = 0.0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public DateTime LastAdaptation { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
