namespace TuneSpace.Api.DTOs;

public record BandCreateRequest(
    string UserId,
    string Name,
    string Description,
    string Genre,
    string Location,
    IFormFile Picture
);
