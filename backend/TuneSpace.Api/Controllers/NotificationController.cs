using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TuneSpace.Api.Extensions;
using TuneSpace.Core.DTOs.Requests.Notification;
using TuneSpace.Core.Interfaces.IClients;
using TuneSpace.Core.Interfaces.IServices;
using TuneSpace.Infrastructure.Hubs;

namespace TuneSpace.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotificationController(
    IHubContext<SocketHub, ISocketClient> hubContext,
    INotificationService notificationService,
    ILogger<NotificationController> logger) : ControllerBase
{
    private readonly IHubContext<SocketHub, ISocketClient> _hubContext = hubContext;
    private readonly INotificationService _notificationService = notificationService;
    private readonly ILogger<NotificationController> _logger = logger;

    [HttpGet("{username}")]
    public async Task<IActionResult> GetNotifications(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return BadRequest("Username cannot be null or empty");
        }

        try
        {
            return Ok(await _notificationService.GetUserNotificationsAsync(username));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications for user");
            return StatusCode(500, "An error occurred while retrieving notifications.");
        }
    }

    [HttpPost("send-notification")]
    public async Task<IActionResult> SendNotification([FromBody] AddNotificationRequestDto request)
    {
        try
        {
            var notificationResponse = await _notificationService.CreateNotificationAsync(request);
            await _hubContext.Clients.Group(request.RecipientName).ReceiveNotification(notificationResponse);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to {RecipientName}", LoggingExtensions.SanitizeForLogging(request?.RecipientName));
            return StatusCode(500, "An error occurred while sending the notification.");
        }
    }

    [HttpPut("mark-as-read/{notificationId}")]
    public async Task<IActionResult> ReadNotification(Guid notificationId)
    {
        if (notificationId == Guid.Empty)
        {
            return BadRequest("Notification ID cannot be empty.");
        }

        try
        {
            await _notificationService.ReadNotificationAsync(notificationId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {NotificationId} as read", notificationId);
            return StatusCode(500, "An error occurred while marking the notification as read.");
        }
    }

    [HttpPut("mark-all-as-read/{username}")]
    public async Task<IActionResult> ReadNotifications(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return BadRequest("Username cannot be null or empty");
        }

        try
        {
            await _notificationService.ReadNotificationsAsync(username);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read for user");
            return StatusCode(500, "An error occurred while marking all notifications as read.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotification(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Notification ID cannot be empty.");
        }

        try
        {
            await _notificationService.DeleteNotificationAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting notification {NotificationId}", id);
            return StatusCode(500, "An error occurred while deleting the notification.");
        }
    }
}
