namespace TuneSpace.Core.Entities;

public class Band
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Genre { get; set; }
    public byte[]? CoverImage { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    // public string? SpotifyId { get; set; }
    // public int SpotifyFollowers { get; set; }
    // public int SpotifyPopularity { get; set; }
}
