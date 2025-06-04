namespace TuneSpace.Core.Entities;

public class BandFollow
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BandId { get; set; }
    public DateTime Timestamp { get; set; }

    public User User { get; set; } = null!;
    public Band Band { get; set; } = null!;
}
