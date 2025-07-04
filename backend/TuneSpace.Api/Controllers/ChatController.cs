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
    IUserService userService,
    ILogger<ChatController> logger) : ControllerBase
{
    private readonly IHubContext<SocketHub, ISocketClient> _hubContext = hubContext;
    private readonly IChatService _chatService = chatService;
    private readonly IUserService _userService = userService;
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
            _logger.LogError(ex, "Error in GetUserChats for username");
            return StatusCode(500, "An error occurred while retrieving user chats.");
        }
    }

    [HttpPost("create-chat")]
    public async Task<IActionResult> CreateChat([FromBody] CreateChatRequest request)
    {
        try
        {
            var chatResponse = await _chatService.CreateChatAsync(request);

            var receiverUser = await _userService.GetUserByNameAsync(request.User2Name);
            if (receiverUser != null)
            {
                await _hubContext.Clients.Group(receiverUser.Id.ToString()).ChatCreated(chatResponse);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateChat for users {User1}, {User2}", LoggingExtensions.SanitizeForLogging(request.User1Name), LoggingExtensions.SanitizeForLogging(request.User2Name));
            return StatusCode(500, "An error occurred while creating chat.");
        }
    }

    [HttpPost("send-private-message")]
    public async Task<IActionResult> SendPrivateMessage([FromBody] SendMessageRequest request)
    {
        try
        {
            var messageResponse = await _chatService.CreateMessageAsync(request);

            var receiverUser = await _userService.GetUserByNameAsync(request.Receiver);
            if (receiverUser != null)
            {
                await _hubContext.Clients.Group(receiverUser.Id.ToString()).ReceiveMessage(messageResponse);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SendPrivateMessage from {Sender} to {Receiver}", LoggingExtensions.SanitizeForLogging(request.Sender), LoggingExtensions.SanitizeForLogging(request.Receiver));
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
            _logger.LogError(ex, "Error in MarkMessagesAsRead for chatId and username");
            return StatusCode(500, "An error occurred while marking messages as read.");
        }
    }
}
