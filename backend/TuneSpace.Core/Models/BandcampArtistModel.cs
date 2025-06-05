namespace TuneSpace.Core.Models;

public class BandcampArtistModel
{
    public string Name { get; set; } = string.Empty;
    public string BandcampUrl { get; set; } = string.Empty;
    public string? Location { get; set; }
    public List<string> Genres { get; set; } = [];
    public string? Description { get; set; }
    public List<string> Tags { get; set; } = [];
    public string? ImageUrl { get; set; }
    public int? Followers { get; set; }
    public List<AlbumModel> Albums { get; set; } = [];
    public List<string> SimilarArtists { get; set; } = [];
    public DateTime? LastActive { get; set; }
    public string? SocialLinks { get; set; }
}

public class AlbumModel
{
    public string Title { get; set; } = string.Empty;
    public string? ReleaseDate { get; set; }
    public string? CoverArtUrl { get; set; }
    public List<string> Tracks { get; set; } = [];
    public decimal? Price { get; set; }
}
