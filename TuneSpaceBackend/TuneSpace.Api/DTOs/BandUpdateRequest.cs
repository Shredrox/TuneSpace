namespace TuneSpace.Api.DTOs;

public class BandUpdateRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string SpotifyId { get; set; } = string.Empty;
    public string YouTubeEmbedId { get; set; } = string.Empty;
    public IFormFile Picture { get; set; } = null!;
}
