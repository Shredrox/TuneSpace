namespace TuneSpace.Core.DTOs.Responses.Chat;

public record ChatResponse(
    Guid Id,
    Guid User1Id,
    string User1Name,
    Guid User2Id,
    string User2Name,
    //add recipient avatar
    string LastMessage,
    DateTime? LastMessageTime,
    int UnreadCount);
