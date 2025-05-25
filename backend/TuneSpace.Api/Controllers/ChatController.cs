using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TuneSpace.Core.DTOs.Requests.Chat;
using TuneSpace.Core.Interfaces.IClients;
using TuneSpace.Core.Interfaces.IServices;
using TuneSpace.Infrastructure.Hubs;

namespace TuneSpace.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController(
    IHubContext<SocketHub, ISocketClient> hubContext,
    IChatService chatService) : ControllerBase
{
    [HttpGet("get-messages/{chatId}")]
    public async Task<IActionResult> GetMessages(Guid chatId)
    {
        return Ok(await chatService.GetChatMessages(chatId));
    }

    [HttpGet("get-chat/{chatId}")]
    public async Task<IActionResult> GetChat(Guid chatId)
    {
        return Ok(await chatService.GetChatById(chatId));
    }

    [HttpGet("get-user-chats")]
    public async Task<IActionResult> GetUserChats([FromQuery] string username)
    {
        return Ok(await chatService.GetUserChats(username));
    }

    [HttpPost("create-chat")]
    public async Task<IActionResult> CreateChat([FromBody] CreateChatRequest request)
    {
        var chatResponse = await chatService.CreateChat(request);

        await hubContext.Clients.Group(request.User2Name).ChatCreated(chatResponse);

        return Ok();
    }

    [HttpPost("send-private-message")]
    public async Task<IActionResult> SendPrivateMessage([FromBody] SendMessageRequest request)
    {
        var messageResponse = await chatService.CreateMessage(request);

        await hubContext.Clients.Group(request.Receiver).ReceiveMessage(messageResponse);

        return Ok();
    }

    [HttpPost("mark-as-read")]
    public async Task<IActionResult> MarkMessagesAsRead([FromBody] MarkMessagesReadRequest request)
    {
        await chatService.MarkMessagesAsRead(request.ChatId, request.Username);
        return Ok();
    }
}
