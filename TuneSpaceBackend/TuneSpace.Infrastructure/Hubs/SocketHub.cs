using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TuneSpace.Core.DTOs.Responses.Chat;
using TuneSpace.Core.Interfaces.IClients;

namespace TuneSpace.Infrastructure.Hubs;

[Authorize]
public class SocketHub : Hub<ISocketClient>
{
    public override async Task OnConnectedAsync()
    {
        if (!string.IsNullOrEmpty(Context.User?.Identity?.Name))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            await base.OnConnectedAsync();
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (!string.IsNullOrEmpty(Context.User?.Identity?.Name))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.User.Identity.Name);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
