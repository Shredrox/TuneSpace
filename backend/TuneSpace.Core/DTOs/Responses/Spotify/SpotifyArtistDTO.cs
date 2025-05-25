namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class SpotifyArtistDTO
{
    public Followers Followers { get; set; }
    public List<string> Genres { get; set; }
    public string Href { get; set; }
    public string Id { get; set; }
    public List<SpotifyImageDTO> Images { get; set; }
    public string Name { get; set; }
    public int Popularity { get; set; }
}
