namespace TuneSpace.Core.DTOs.Requests.Notification;

public record AddNotificationRequestDto(
    string Message,
    string Type,
    string Source,
    string RecipientName);
