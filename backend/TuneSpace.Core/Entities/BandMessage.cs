namespace TuneSpace.Core.Entities;

public class BandMessage
{
    public Guid Id { get; set; }
    public Guid BandChatId { get; set; }
    public Guid? SenderId { get; set; }
    public Guid? BandId { get; set; }
    public required string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsRead { get; set; }
    public bool IsFromBand { get; set; }

    public BandChat BandChat { get; set; } = null!;
    public User? Sender { get; set; }
    public Band? Band { get; set; }
}
