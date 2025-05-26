using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IRepositories;

/// <summary>
/// Provides data access methods for managing chats in the TuneSpace application.
/// </summary>
public interface IChatRepository
{
    /// <summary>
    /// Retrieves a chat by its unique identifier.
    /// </summary>
    /// <param name="chatId">The unique identifier of the chat to retrieve.</param>
    /// <returns>A task representing the asynchronous operation, containing the chat if found, or null if not found.</returns>
    Task<Chat?> GetChatByIdAsync(Guid chatId);

    /// <summary>
    /// Gets all chats where either user1 or user2 is a participant.
    /// </summary>
    /// <param name="user1">The first user to check for participation in chats.</param>
    /// <param name="user2">The second user to check for participation in chats.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of chat entities.</returns>
    Task<List<Chat>> GetChatsByUser1IdOrUser2IdAsync(User user1, User user2);

    /// <summary>
    /// Retrieves a specific chat between two users.
    /// </summary>
    /// <param name="user1">The first user in the chat.</param>
    /// <param name="user2">The second user in the chat.</param>
    /// <returns>A task representing the asynchronous operation, containing the chat if found, or null if not found.</returns>
    Task<Chat?> GetChatByUser1AndUser2Async(User user1, User user2);

    /// <summary>
    /// Inserts a new chat into the repository.
    /// </summary>
    /// <param name="chat">The chat entity to insert.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InsertChatAsync(Chat chat);
}
