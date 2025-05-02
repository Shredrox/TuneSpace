namespace TuneSpace.Core.Entities;

public class Favorite
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    //make it enum
    public string Type { get; set; }
}
