namespace TuneSpace.Core.DTOs.Requests.Band;

public record AddMemberRequest(
    string BandId,
    string UserId);
