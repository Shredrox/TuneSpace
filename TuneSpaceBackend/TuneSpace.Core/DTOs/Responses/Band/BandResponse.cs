namespace TuneSpace.Core.DTOs.Responses.Band;

public record BandResponse(
    string Name,
    string Description,
    string Genre,
    string Country,
    string City,
    byte[] CoverImage);
