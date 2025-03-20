namespace TuneSpace.Core.DTOs.Requests.Band;

public class UpdateBandRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Genre { get; set; }
    public string Description { get; set; }
}