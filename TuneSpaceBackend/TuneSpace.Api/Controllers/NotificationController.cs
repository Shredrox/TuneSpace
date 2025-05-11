using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TuneSpace.Core.DTOs.Requests.Notification;
using TuneSpace.Core.Interfaces.IClients;
using TuneSpace.Core.Interfaces.IServices;
using TuneSpace.Infrastructure.Hubs;

namespace TuneSpace.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController(
    IHubContext<SocketHub, ISocketClient> hubContext,
    INotificationService notificationService) : ControllerBase
{
    [HttpGet("{username}")]
    public async Task<IActionResult> GetNotifications(string username)
    {
        return Ok(await notificationService.GetUserNotifications(username));
    }

    [HttpPost("send-notification")]
    public async Task<IActionResult> SendNotification([FromBody] AddNotificationRequestDto request)
    {
        var notificationResponse = await notificationService.CreateNotification(request);

        await hubContext.Clients.Group(request.RecipientName).ReceiveNotification(notificationResponse);

        return Ok();
    }

    [HttpPut("mark-as-read/{notificationId}")]
    public async Task<IActionResult> ReadNotification(Guid notificationId)
    {
        await notificationService.ReadNotification(notificationId);
        return Ok();
    }

    [HttpPut("mark-all-as-read/{username}")]
    public async Task<IActionResult> ReadNotifications(string username)
    {
        await notificationService.ReadNotifications(username);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotification(Guid id)
    {
        await notificationService.DeleteNotification(id);
        return Ok();
    }
}
