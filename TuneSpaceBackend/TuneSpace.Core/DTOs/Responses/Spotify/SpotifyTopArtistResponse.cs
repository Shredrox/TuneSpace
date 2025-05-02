namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class SpotifyTopArtistResponse
{
    public string Name { get; set; }
    public int Popularity { get; set; }
    public List<SpotifyImageDTO> Images { get; set; }
}
