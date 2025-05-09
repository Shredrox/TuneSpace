namespace TuneSpace.Core.Interfaces.IClients;

public interface ISocketClient
{
    Task ReceiveMessage(string user, string message);
}
