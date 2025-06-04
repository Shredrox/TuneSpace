using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TuneSpace.Core.Interfaces.IClients;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Infrastructure.Hubs;

[Authorize]
public class SocketHub(IBandService bandService) : Hub<ISocketClient>
{
    private readonly IBandService _bandService = bandService;

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);

            await AddUserToBandGroups(userId);

            await base.OnConnectedAsync();
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);

            await RemoveUserFromBandGroups(userId);

            await base.OnDisconnectedAsync(exception);
        }
    }

    public async Task JoinBandGroup(string bandId)
    {
        if (!string.IsNullOrEmpty(bandId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"band_{bandId}");
        }
    }

    public async Task LeaveBandGroup(string bandId)
    {
        if (!string.IsNullOrEmpty(bandId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"band_{bandId}");
        }
    }

    private async Task AddUserToBandGroups(string userId)
    {
        try
        {
            var userBand = await _bandService.GetBandByUserIdAsync(userId);
            if (userBand != null)
            {
                if (Guid.TryParse(userBand.Id, out var bandId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"band_{bandId}");
                }
            }

            // Note: If bands can have multiple members who aren't owners,
            // a different method would be needed to get all bands a user is a member of
            // For now, each user can only be in one band
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding user to band groups: {ex.Message}");
        }
    }

    private async Task RemoveUserFromBandGroups(string userId)
    {
        try
        {
            var userBand = await _bandService.GetBandByUserIdAsync(userId);
            if (userBand != null)
            {
                if (Guid.TryParse(userBand.Id, out var bandId))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"band_{bandId}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing user from band groups: {ex.Message}");
        }
    }
}
