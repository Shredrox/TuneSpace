namespace TuneSpace.Core.DTOs.Requests.MusicEvent;

public record CreateMusicEventRequest(
    Guid BandId,
    string Title,
    string? Description,
    DateTime EventDate,
    string? Location,
    string? VenueAddress,
    decimal? TicketPrice,
    string? TicketUrl,
    string? City,
    string? Country);
