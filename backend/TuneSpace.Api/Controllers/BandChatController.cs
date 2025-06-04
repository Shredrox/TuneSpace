using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TuneSpace.Api.Extensions;
using TuneSpace.Core.DTOs.Requests.Chat;
using TuneSpace.Core.Interfaces.IClients;
using TuneSpace.Core.Interfaces.IServices;
using TuneSpace.Infrastructure.Hubs;

namespace TuneSpace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BandChatController(
    IBandChatService bandChatService,
    IBandMessageService bandMessageService,
    IHubContext<SocketHub, ISocketClient> hubContext,
    ILogger<BandChatController> logger) : ControllerBase
{
    private readonly IBandChatService _bandChatService = bandChatService;
    private readonly IBandMessageService _bandMessageService = bandMessageService;
    private readonly IHubContext<SocketHub, ISocketClient> _hubContext = hubContext;
    private readonly ILogger<BandChatController> _logger = logger;

    [HttpGet("user-chats")]
    public async Task<IActionResult> GetUserBandChats()
    {
        try
        {
            var userId = User.GetUserId();
            var chats = await _bandChatService.GetUserBandChatsAsync(userId);
            return Ok(chats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user band chats");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("band-chats/{bandId:guid}")]
    public async Task<IActionResult> GetBandChats(Guid bandId)
    {
        try
        {
            var chats = await _bandChatService.GetBandChatsAsync(bandId);
            return Ok(chats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting chats for band {BandId}", bandId);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{chatId:guid}/messages")]
    public async Task<IActionResult> GetChatMessages(Guid chatId, [FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        try
        {
            var messages = await _bandMessageService.GetChatMessagesAsync(chatId, skip, take);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting messages for chat {ChatId}", chatId);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{chatId:guid}/unread-count")]
    public async Task<IActionResult> GetUnreadCount(Guid chatId)
    {
        try
        {
            var userId = User.GetUserId();
            var count = await _bandMessageService.GetUnreadCountAsync(userId, chatId);
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread count for chat {ChatId}", chatId);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{chatId:guid}")]
    public async Task<IActionResult> GetBandChatById(Guid chatId)
    {
        try
        {
            var bandChat = await _bandChatService.GetBandChatByIdAsync(chatId);
            if (bandChat == null)
            {
                return NotFound();
            }
            return Ok(bandChat);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting band chat {ChatId}", chatId);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("check/{bandId:guid}")]
    public async Task<IActionResult> CheckBandChat(Guid bandId)
    {
        try
        {
            var userId = User.GetUserId();
            var bandChat = await _bandChatService.GetBandChatAsync(userId, bandId);

            if (bandChat == null)
            {
                return NotFound();
            }

            return Ok(bandChat);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking band chat for band {BandId}", bandId);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("start/{bandId:guid}")]
    public async Task<IActionResult> StartBandChat(Guid bandId)
    {
        try
        {
            var userId = User.GetUserId();
            var bandChat = await _bandChatService.CreateOrGetBandChatAsync(userId, bandId);

            await _hubContext.Clients.Group($"band_{bandId}").BandChatCreated(bandChat);

            return Ok(bandChat);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting band chat with band {BandId}", bandId);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{bandId:guid}/send-to-band")]
    public async Task<IActionResult> SendMessageToBand(Guid bandId, [FromBody] SendBandMessageRequest request)
    {
        try
        {
            var userId = User.GetUserId();
            var message = await _bandMessageService.SendMessageToBandAsync(userId, bandId, request.Content);

            await _hubContext.Clients.Group($"band_{bandId}").ReceiveBandMessage(message);

            return Ok(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message to band {BandId}", bandId);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{bandId:guid}/send-from-band/{userId:guid}")]
    public async Task<IActionResult> SendMessageFromBand(Guid bandId, Guid userId, [FromBody] SendBandMessageRequest request)
    {
        try
        {
            var bandMemberId = User.GetUserId();
            var message = await _bandMessageService.SendMessageFromBandAsync(bandId, userId, request.Content, bandMemberId);

            await _hubContext.Clients.User(userId.ToString()).ReceiveBandMessage(message);
            await _hubContext.Clients.Group($"band_{bandId}").ReceiveBandMessage(message);

            return Ok(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message from band {BandId} to user {UserId}", bandId, userId);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("messages/{messageId:guid}/read")]
    public async Task<IActionResult> MarkMessageAsRead(Guid messageId)
    {
        try
        {
            var userId = User.GetUserId();
            await _bandMessageService.MarkMessageAsReadAsync(messageId, userId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking message {MessageId} as read", messageId);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("messages/{messageId:guid}")]
    public async Task<IActionResult> DeleteMessage(Guid messageId)
    {
        try
        {
            var userId = User.GetUserId();
            await _bandMessageService.DeleteMessageAsync(messageId, userId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting message {MessageId}", messageId);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{chatId:guid}")]
    public async Task<IActionResult> DeleteBandChat(Guid chatId)
    {
        try
        {
            var userId = User.GetUserId();
            await _bandChatService.DeleteBandChatAsync(chatId, userId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting band chat {ChatId}", chatId);
            return BadRequest(new { message = ex.Message });
        }
    }
}
