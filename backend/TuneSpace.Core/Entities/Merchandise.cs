namespace TuneSpace.Core.Entities;

public class Merchandise
{
    public Guid Id { get; set; }
    public Guid BandId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public byte[]? Image { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Band Band { get; set; } = null!;
}
