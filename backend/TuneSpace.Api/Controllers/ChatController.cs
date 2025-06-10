using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TuneSpace.Api.Extensions;
using TuneSpace.Core.DTOs.Requests.Chat;
using TuneSpace.Core.Interfaces.IClients;
using TuneSpace.Core.Interfaces.IServices;
using TuneSpace.Infrastructure.Hubs;

namespace TuneSpace.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ChatController(
    IHubContext<SocketHub, ISocketClient> hubContext,
    IChatService chatService,
    ILogger<ChatController> logger) : ControllerBase
{
    private readonly IHubContext<SocketHub, ISocketClient> _hubContext = hubContext;
    private readonly IChatService _chatService = chatService;
    private readonly ILogger<ChatController> _logger = logger;

    [HttpGet("get-messages/{chatId}")]
    public async Task<IActionResult> GetMessages(Guid chatId)
    {
        if (chatId == Guid.Empty)
        {
            return BadRequest("Chat ID cannot be empty.");
        }

        try
        {
            return Ok(await _chatService.GetChatMessagesAsync(chatId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetMessages for chatId {ChatId}", chatId);
            return StatusCode(500, "An error occurred while retrieving messages.");
        }
    }

    [HttpGet("get-chat/{chatId}")]
    public async Task<IActionResult> GetChat(Guid chatId)
    {
        if (chatId == Guid.Empty)
        {
            return BadRequest("Chat ID cannot be empty.");
        }

        try
        {
            return Ok(await _chatService.GetChatByIdAsync(chatId));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetChat for chatId {ChatId}", chatId);
            return StatusCode(500, "An error occurred while retrieving chat.");
        }
    }

    [HttpGet("get-user-chats")]
    public async Task<IActionResult> GetUserChats([FromQuery] string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return BadRequest("Username cannot be null or empty.");
        }

        try
        {
            return Ok(await _chatService.GetUserChatsAsync(username));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetUserChats for username {Username}", username.SanitizeForLogging());
            return StatusCode(500, "An error occurred while retrieving user chats.");
        }
    }

    [HttpPost("create-chat")]
    public async Task<IActionResult> CreateChat([FromBody] CreateChatRequest request)
    {
        try
        {
            var chatResponse = await _chatService.CreateChatAsync(request);

            await _hubContext.Clients.Group(request.User2Name).ChatCreated(chatResponse);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateChat for users {User1}, {User2}", request.User1Name, request.User2Name);
            return StatusCode(500, "An error occurred while creating chat.");
        }
    }

    [HttpPost("send-private-message")]
    public async Task<IActionResult> SendPrivateMessage([FromBody] SendMessageRequest request)
    {
        try
        {
            var messageResponse = await _chatService.CreateMessageAsync(request);

            await _hubContext.Clients.Group(request.Receiver).ReceiveMessage(messageResponse);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SendPrivateMessage from {Sender} to {Receiver}", request.Sender, request.Receiver);
            return StatusCode(500, "An error occurred while sending message.");
        }
    }

    [HttpPost("mark-as-read")]
    public async Task<IActionResult> MarkMessagesAsRead([FromBody] MarkMessagesReadRequest request)
    {
        try
        {
            await _chatService.MarkMessagesAsReadAsync(request.ChatId, request.Username);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in MarkMessagesAsRead for chatId {ChatId} and username {Username}", request.ChatId, request.Username.SanitizeForLogging());
            return StatusCode(500, "An error occurred while marking messages as read.");
        }
    }
}
