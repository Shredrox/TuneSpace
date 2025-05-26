using TuneSpace.Core.DTOs.Requests.Chat;
using TuneSpace.Core.DTOs.Responses.Chat;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Provides business logic services for managing chats in the TuneSpace application.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Retrieves all messages from a specific chat.
    /// </summary>
    /// <param name="chatId">The unique identifier of the chat to retrieve messages from.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of messages in the chat.</returns>
    Task<List<MessageResponse>> GetChatMessagesAsync(Guid chatId);

    /// <summary>
    /// Retrieves a specific chat by its unique identifier.
    /// </summary>
    /// <param name="chatId">The unique identifier of the chat to retrieve.</param>
    /// <returns>A task representing the asynchronous operation, containing the requested chat information.</returns>
    Task<ChatResponse> GetChatByIdAsync(Guid chatId);

    /// <summary>
    /// Retrieves all chats that a user is participating in.
    /// </summary>
    /// <param name="username">The username of the user whose chats to retrieve.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of chats the user is participating in.</returns>
    Task<List<ChatResponse>> GetUserChatsAsync(string username);

    /// <summary>
    /// Creates a new chat between users.
    /// </summary>
    /// <param name="request">The request containing information needed to create a chat.</param>
    /// <returns>A task representing the asynchronous operation, containing the created chat information.</returns>
    Task<ChatResponse> CreateChatAsync(CreateChatRequest request);

    /// <summary>
    /// Creates and sends a new message in a chat.
    /// </summary>
    /// <param name="request">The request containing the message content and associated chat information.</param>
    /// <returns>A task representing the asynchronous operation, containing the sent message information.</returns>
    Task<MessageResponse> CreateMessageAsync(SendMessageRequest request);

    /// <summary>
    /// Marks messages in a chat as read by the user.
    /// </summary>
    /// <param name="chatId">The unique identifier of the chat.</param>
    /// <param name="username">The username of the user marking the messages as read.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task MarkMessagesAsReadAsync(Guid chatId, string username);
}
