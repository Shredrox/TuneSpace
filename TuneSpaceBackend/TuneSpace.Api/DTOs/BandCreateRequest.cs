namespace TuneSpace.Api.DTOs;

public class BandCreateRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public IFormFile Picture { get; set; } = null!;
}
