namespace TuneSpace.Core.Entities;

public class Message
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public string SenderId { get; set; }
    public User Sender { get; set; }
    public string ReceiverId { get; set; }
    public User Receiver { get; set; }
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }
    public DateTime Timestamp { get; set; }
}
