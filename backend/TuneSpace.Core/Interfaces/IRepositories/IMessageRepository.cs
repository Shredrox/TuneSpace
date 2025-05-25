using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IRepositories;

/// <summary>
/// Provides data access methods for managing messages in the TuneSpace application.
/// </summary>
public interface IMessageRepository
{
    /// <summary>
    /// Inserts a new message into the repository.
    /// </summary>
    /// <param name="message">The message entity to insert.</param>
    /// <returns>A task representing the asynchronous operation, containing the inserted message with any generated values (e.g. ID).</returns>
    Task<Message> InsertMessage(Message message);

    /// <summary>
    /// Retrieves all messages exchanged between two users.
    /// </summary>
    /// <param name="userId">The ID of the first user.</param>
    /// <param name="otherUserId">The ID of the second user.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of messages exchanged between the users.</returns>
    Task<List<Message>> GetMessagesBetweenUsersAsync(Guid userId, Guid otherUserId);

    /// <summary>
    /// Retrieves all messages in a specific chat.
    /// </summary>
    /// <param name="chatId">The ID of the chat to retrieve messages from.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of messages in the chat.</returns>
    Task<List<Message>> GetMessagesByChatIdAsync(Guid chatId);

    /// <summary>
    /// Gets the IDs of all users who have exchanged messages with the specified user.
    /// </summary>
    /// <param name="userId">The ID of the user to find conversation partners for.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of user IDs.</returns>
    Task<List<Guid>> GetConversationPartnerIdsAsync(Guid userId);

    /// <summary>
    /// Retrieves the most recent message exchanged between two users.
    /// </summary>
    /// <param name="userId">The ID of the first user.</param>
    /// <param name="partnerId">The ID of the second user.</param>
    /// <returns>A task representing the asynchronous operation, containing the latest message if found, or null if no messages exist.</returns>
    Task<Message?> GetLatestMessageBetweenUsersAsync(Guid userId, Guid partnerId);

    /// <summary>
    /// Gets the count of unread messages sent by a specific user to a recipient in a chat.
    /// </summary>
    /// <param name="senderId">The ID of the sender.</param>
    /// <param name="recipientId">The ID of the recipient.</param>
    /// <param name="chatId">The ID of the chat.</param>
    /// <returns>A task representing the asynchronous operation, containing the count of unread messages.</returns>
    Task<int> GetUnreadMessageCountFromUserAsync(Guid senderId, Guid recipientId, Guid chatId);

    /// <summary>
    /// Retrieves all unread messages sent by a specific user to a recipient.
    /// </summary>
    /// <param name="senderId">The ID of the sender.</param>
    /// <param name="recipientId">The ID of the recipient.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of unread messages.</returns>
    Task<List<Message>> GetUnreadMessagesAsync(Guid senderId, Guid recipientId);

    /// <summary>
    /// Gets the total count of unread messages for a user across all conversations.
    /// </summary>
    /// <param name="userId">The ID of the user to check unread messages for.</param>
    /// <returns>A task representing the asynchronous operation, containing the count of total unread messages.</returns>
    Task<int> GetTotalUnreadMessageCountAsync(Guid userId);

    /// <summary>
    /// Updates a collection of messages in the repository.
    /// </summary>
    /// <param name="messages">The collection of messages to update.</param>
    /// <returns>A task representing the asynchronous operation, containing a boolean indicating success or failure.</returns>
    Task<bool> UpdateMessagesAsync(IEnumerable<Message> messages);

    /// <summary>
    /// Marks messages as read in the repository.
    /// </summary>
    /// <param name="chatId">The ID of the chat containing the messages to mark as read.</param>
    /// <param name="senderId">The ID of the sender of the messages.</param>
    /// <param name="recipientId">The ID of the recipient of the messages.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task MarkMessagesAsReadAsync(Guid chatId, string senderId, string recipientId);
}
