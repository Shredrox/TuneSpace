namespace TuneSpace.Core.Entities;

public class Song
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid FavoriteId { get; set; }
    public Favorite Favorite { get; set; }
    public List<Playlist> Playlists { get; set; }
}