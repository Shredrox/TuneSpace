namespace TuneSpace.Core.Entities;

public class Message
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public Guid SenderId { get; set; }
    public User Sender { get; set; }
    public Guid RecipientId { get; set; }
    public User Recipient { get; set; }
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsRead { get; set; }
}
