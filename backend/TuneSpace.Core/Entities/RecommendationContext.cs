using Pgvector;

namespace TuneSpace.Core.Entities;

public class RecommendationContext
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<string> UserGenres { get; set; } = [];
    public string? UserLocation { get; set; }
    public List<string> UserTopArtists { get; set; } = [];
    public List<string> UserRecentlyPlayed { get; set; } = [];
    public Vector? UserPreferenceEmbedding { get; set; }
    public string? RetrievedContext { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
