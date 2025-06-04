namespace TuneSpace.Core.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Message { get; set; }
    public string? Type { get; set; }
    public string? Source { get; set; }
    public required string RecipientName { get; set; }
    public bool IsRead { get; set; }
    public DateTime Timestamp { get; set; }

    public User User { get; set; } = null!;
}
