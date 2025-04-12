namespace TuneSpace.Core.Interfaces.IClients;

public interface IChatClient
{
    Task ReceiveMessage(string user, string message);
}
