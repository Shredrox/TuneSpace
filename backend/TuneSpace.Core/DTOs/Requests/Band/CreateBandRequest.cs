namespace TuneSpace.Core.DTOs.Requests.Band;

public record CreateBandRequest(
    string UserId,
    string Name,
    string Description,
    string Genre,
    string Location,
    FileDto? Picture);
