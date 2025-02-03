namespace TuneSpace.Core.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public Guid PostId { get; set; }
    public Post Post { get; set; }
    public DateTime Timestamp { get; set; }
}