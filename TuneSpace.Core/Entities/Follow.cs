namespace TuneSpace.Core.Entities;

public class Follow
{
    public Guid Id { get; set; }
    public string FollowerId { get; set; }
    public User Follower { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public DateTime Timestamp { get; set; }
}
