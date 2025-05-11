namespace TuneSpace.Core.DTOs.Responses.Notification;

public record NotificationResponseDto(
    Guid Id,
    string Message,
    bool IsRead,
    string Type,
    DateTime Timestamp);
