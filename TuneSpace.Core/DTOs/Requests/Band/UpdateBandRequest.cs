using Microsoft.AspNetCore.Http;

namespace TuneSpace.Core.DTOs.Requests.Band;

public record UpdateBandRequest(
    Guid Id,
    string? Name,
    string? Genre,
    string? Description,
    string? SpotifyId,
    string? YouTubeEmbedId,
    IFormFile? Picture);
