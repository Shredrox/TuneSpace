namespace TuneSpace.Core.DTOs.Requests.MusicEvent;

public record UpdateMusicEventRequest(
    Guid Id,
    string? Title,
    string? Description,
    DateTime? EventDate,
    string? Location,
    string? VenueAddress,
    decimal? TicketPrice,
    string? TicketUrl,
    bool? IsCancelled);
