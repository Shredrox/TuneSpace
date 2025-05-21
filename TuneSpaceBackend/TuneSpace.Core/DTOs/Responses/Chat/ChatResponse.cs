namespace TuneSpace.Core.DTOs.Responses.Chat;

public record ChatResponse(
    Guid Id,
    Guid User1Id,
    string User1Name,
    byte[] User1Avatar,
    Guid User2Id,
    string User2Name,
    byte[] User2Avatar,
    string LastMessage,
    DateTime? LastMessageTime,
    int UnreadCount);
