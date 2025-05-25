namespace TuneSpace.Core.Entities;

public class Band
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Genre { get; set; }
    public byte[]? CoverImage { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? SpotifyId { get; set; }
    public string? YouTubeEmbedId { get; set; }

    public ICollection<User> Members { get; set; } = [];
    public ICollection<MusicEvent> Events { get; set; } = [];
    public ICollection<Merchandise> Merchandise { get; set; } = [];
}
