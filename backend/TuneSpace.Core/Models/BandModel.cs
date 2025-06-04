using System.Text.Json.Serialization;

namespace TuneSpace.Core.Models;

public class BandModel
{
    public string? Id { get; set; } = null;
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public List<string> Genres { get; set; } = new();
    public int Listeners { get; set; }
    public int PlayCount { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public double Popularity { get; set; }
    public double RelevanceScore { get; set; }
    public double DiversityScore { get; set; }
    public List<string> SimilarArtists { get; set; } = new();
    public bool IsRegistered { get; set; } = false;

    [JsonIgnore]
    public string? SimilarToArtistName { get; set; }

    [JsonIgnore]
    public string? SimilarToRegisteredBand { get; set; }

    [JsonIgnore]
    public bool IsLesserKnown => Listeners < 100000 || Popularity < 40;

    public bool IsFromSearch { get; set; }
    public bool IsNewRelease { get; set; }
    public List<string> SearchTags { get; set; } = new();

    public string? LatestAlbum { get; set; }
    public string? LatestAlbumReleaseDate { get; set; }
}
