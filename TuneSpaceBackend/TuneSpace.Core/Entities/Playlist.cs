namespace TuneSpace.Core.Entities;

public class Playlist
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public List<Song> Songs { get; set; }
}
