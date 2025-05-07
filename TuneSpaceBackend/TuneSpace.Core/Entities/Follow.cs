namespace TuneSpace.Core.Entities;

public class Follow
{
    public Guid Id { get; set; }
    public Guid FollowerId { get; set; }
    public User Follower { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public DateTime Timestamp { get; set; }
}
