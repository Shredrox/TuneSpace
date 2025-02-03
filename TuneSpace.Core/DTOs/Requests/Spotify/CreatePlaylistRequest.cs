namespace TuneSpace.Core.DTOs.Requests.Spotify;

public class CreatePlaylistRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Public { get; set; }
    public string UserId { get; set; }
}