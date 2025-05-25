namespace TuneSpace.Core.Entities;

public class MusicEvent
{
    public Guid Id { get; set; }
    public Guid BandId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Location { get; set; }
    public string? VenueAddress { get; set; }
    public decimal? TicketPrice { get; set; }
    public string? TicketUrl { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Band Band { get; set; } = null!;
}
