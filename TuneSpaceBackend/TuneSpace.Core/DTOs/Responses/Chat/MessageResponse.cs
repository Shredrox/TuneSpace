namespace TuneSpace.Core.DTOs.Responses.Chat;

public record MessageResponse(
    Guid Id,
    string Content,
    Guid SenderId,
    string SenderName,
    Guid RecipientId,
    string RecipientName,
    bool IsRead,
    DateTime Timestamp);
