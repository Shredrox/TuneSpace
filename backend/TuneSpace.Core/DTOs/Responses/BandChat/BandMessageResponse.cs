namespace TuneSpace.Core.DTOs.Responses.BandChat;

public record BandMessageResponse(
    Guid Id,
    Guid BandChatId,
    string Content,
    DateTime Timestamp,
    bool IsRead,
    bool IsFromBand,
    Guid? SenderId,
    string? SenderName,
    byte[]? SenderAvatar,
    Guid? BandId,
    string? BandName,
    byte[]? BandAvatar);
