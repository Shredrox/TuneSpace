namespace TuneSpace.Core.DTOs.Requests.Chat;

public record MarkMessagesReadRequest(Guid ChatId, string Username);
