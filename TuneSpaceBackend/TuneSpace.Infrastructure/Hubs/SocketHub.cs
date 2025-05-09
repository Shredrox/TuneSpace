using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TuneSpace.Core.DTOs.Responses.Message;
using TuneSpace.Core.Interfaces.IClients;

namespace TuneSpace.Infrastructure.Hubs;

[Authorize]
public class SocketHub : Hub<ISocketClient>
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("sub")?.Value ?? Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            await base.OnConnectedAsync();
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst("sub")?.Value ?? Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            await base.OnDisconnectedAsync(exception);
        }
    }

    public async Task SendMessage(string user, string message)
    {
        await Clients.All.ReceiveMessage(user, message);
    }

    public async Task SendPrivateMessage(string recipientId, MessageResponse message)
    {
        await Clients.Group(recipientId).ReceiveMessage(message.SenderName, message.Content);
    }

    public async Task SendMessageToGroup(string sender, string receiver, string message)
    {
        await Clients.Group(receiver).ReceiveMessage(sender, message);
    }
}
