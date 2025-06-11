using System.ComponentModel.DataAnnotations;
using Pgvector;

namespace TuneSpace.Core.Entities;

public class ArtistEmbedding
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string ArtistName { get; set; } = string.Empty;

    public string? SpotifyId { get; set; }

    public string? BandcampUrl { get; set; }

    public List<string> Genres { get; set; } = [];

    public string? Location { get; set; }

    public string? Description { get; set; }

    public string? Tags { get; set; }

    public Vector? Embedding { get; set; }

    public int? Followers { get; set; }

    public decimal? Popularity { get; set; }

    public DateTime? LastActive { get; set; }

    public string? ImageUrl { get; set; }

    public string? SimilarArtists { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string DataSource { get; set; } = "Bandcamp";

    public string? SourceMetadata { get; set; }
}
