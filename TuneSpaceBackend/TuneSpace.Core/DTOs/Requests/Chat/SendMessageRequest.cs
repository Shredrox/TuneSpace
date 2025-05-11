namespace TuneSpace.Core.DTOs.Requests.Chat;

public record SendMessageRequest(
    string Content,
    string Sender,
    string Receiver);
