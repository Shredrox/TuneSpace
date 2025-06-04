namespace TuneSpace.Core.Entities;

public class Message
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid RecipientId { get; set; }
    public Guid ChatId { get; set; }
    public required string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsRead { get; set; }

    public User Sender { get; set; } = null!;
    public User Recipient { get; set; } = null!;
    public Chat Chat { get; set; } = null!;
}
