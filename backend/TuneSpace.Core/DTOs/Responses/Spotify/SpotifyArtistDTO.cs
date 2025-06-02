namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class SpotifyArtistDTO
{
    public Followers Followers { get; set; } = new();
    public List<string> Genres { get; set; } = [];
    public string Href { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public List<SpotifyImageDTO> Images { get; set; } = [];
    public string Name { get; set; } = string.Empty;
    public int Popularity { get; set; }
}
