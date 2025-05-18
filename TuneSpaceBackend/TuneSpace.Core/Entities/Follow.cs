namespace TuneSpace.Core.Entities;

public class Follow
{
    public Guid Id { get; set; }
    public Guid FollowerId { get; set; }
    public Guid UserId { get; set; }
    public DateTime Timestamp { get; set; }

    public User Follower { get; set; } = null!;
    public User User { get; set; } = null!;
}
