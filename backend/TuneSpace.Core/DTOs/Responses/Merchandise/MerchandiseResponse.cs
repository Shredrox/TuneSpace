namespace TuneSpace.Core.DTOs.Responses.Merchandise;

public record MerchandiseResponse(
    string Id,
    string BandId,
    string Name,
    string Description,
    decimal Price,
    byte[]? Image,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
