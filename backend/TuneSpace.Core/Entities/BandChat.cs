namespace TuneSpace.Core.Entities;

public class BandChat
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BandId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastMessageAt { get; set; }

    public User User { get; set; } = null!;
    public Band Band { get; set; } = null!;
    public ICollection<BandMessage> Messages { get; set; } = [];
}
