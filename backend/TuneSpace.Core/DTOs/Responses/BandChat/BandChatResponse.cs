namespace TuneSpace.Core.DTOs.Responses.BandChat;

public record BandChatResponse(
    Guid Id,
    Guid UserId,
    string UserName,
    byte[] UserAvatar,
    Guid BandId,
    string BandName,
    byte[] BandAvatar,
    string LastMessage,
    DateTime? LastMessageTime,
    int UnreadCount,
    DateTime CreatedAt);
