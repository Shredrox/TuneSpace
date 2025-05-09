namespace TuneSpace.Api.DTOs;

public record BandUpdateRequest(
    Guid Id,
    string? Name,
    string? Genre,
    string? Description,
    string? SpotifyId,
    string? YouTubeEmbedId,
    IFormFile? Picture);
