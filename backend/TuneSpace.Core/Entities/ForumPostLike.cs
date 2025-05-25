namespace TuneSpace.Core.Entities;

public class ForumPostLike
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public ForumPost Post { get; set; } = null!;
    public User User { get; set; } = null!;
}
