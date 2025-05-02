namespace TuneSpace.Core.DTOs.Responses.MusicEvent;

public record MusicEventResponse(
    Guid Id,
    Guid BandId,
    string BandName,
    string Title,
    string? Description,
    DateTime EventDate,
    string? Location,
    string? VenueAddress,
    decimal? TicketPrice,
    string? TicketUrl,
    bool IsCancelled,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
