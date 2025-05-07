namespace TuneSpace.Core.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public DateTime Timestamp { get; set; }
}
