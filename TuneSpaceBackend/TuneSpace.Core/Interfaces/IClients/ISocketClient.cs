using TuneSpace.Core.DTOs.Responses.Notification;

namespace TuneSpace.Core.Interfaces.IClients;

public interface ISocketClient
{
    Task ReceiveMessage(string user, string message);
    Task ReceiveNotification(NotificationResponseDto notification);
}
