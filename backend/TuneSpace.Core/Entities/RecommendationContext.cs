using System.ComponentModel.DataAnnotations;
using Pgvector;

namespace TuneSpace.Core.Entities;

public class RecommendationContext
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string UserId { get; set; } = string.Empty;

    public List<string> UserGenres { get; set; } = [];

    public string? UserLocation { get; set; }

    public List<string> UserTopArtists { get; set; } = [];

    public List<string> UserRecentlyPlayed { get; set; } = [];

    public Vector? UserPreferenceEmbedding { get; set; }

    public string? RetrievedContext { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
