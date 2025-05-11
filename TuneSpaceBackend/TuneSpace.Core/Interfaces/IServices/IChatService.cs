using TuneSpace.Core.DTOs.Requests.Chat;
using TuneSpace.Core.DTOs.Responses.Chat;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Provides business logic services for managing chats in the TuneSpace application.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Creates a new chat between users.
    /// </summary>
    /// <param name="request">The request containing information needed to create a chat.</param>
    /// <returns>A task representing the asynchronous operation, containing the created chat information.</returns>
    Task<ChatResponse> CreateChat(CreateChatRequest request);

    /// <summary>
    /// Creates and sends a new message in a chat.
    /// </summary>
    /// <param name="request">The request containing the message content and associated chat information.</param>
    /// <returns>A task representing the asynchronous operation, containing the sent message information.</returns>
    Task<MessageResponse> CreateMessage(SendMessageRequest request);

    /// <summary>
    /// Retrieves all messages from a specific chat.
    /// </summary>
    /// <param name="chatId">The unique identifier of the chat to retrieve messages from.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of messages in the chat.</returns>
    Task<List<MessageResponse>> GetChatMessages(Guid chatId);

    /// <summary>
    /// Retrieves a specific chat by its unique identifier.
    /// </summary>
    /// <param name="chatId">The unique identifier of the chat to retrieve.</param>
    /// <returns>A task representing the asynchronous operation, containing the requested chat information.</returns>
    Task<ChatResponse> GetChatById(Guid chatId);

    /// <summary>
    /// Retrieves all chats that a user is participating in.
    /// </summary>
    /// <param name="username">The username of the user whose chats to retrieve.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of chats the user is participating in.</returns>
    Task<List<ChatResponse>> GetUserChats(string username);
}
