using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IRepositories;

/// <summary>
/// Repository interface for managing band message operations in the data layer.
/// </summary>
public interface IBandMessageRepository
{
    /// <summary>
    /// Retrieves a band message by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the band message.</param>
    /// <returns>The band message if found, otherwise null.</returns>
    Task<BandMessage?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves messages from a specific band chat with pagination support.
    /// </summary>
    /// <param name="bandChatId">The unique identifier of the band chat.</param>
    /// <param name="skip">The number of messages to skip (default: 0).</param>
    /// <param name="take">The number of messages to retrieve (default: 50).</param>
    /// <returns>A collection of band messages from the specified chat.</returns>
    Task<IEnumerable<BandMessage>> GetChatMessagesAsync(Guid bandChatId, int skip, int take);

    /// <summary>
    /// Gets the count of unread messages for a user in a specific band chat.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="bandChatId">The unique identifier of the band chat.</param>
    /// <returns>The number of unread messages.</returns>
    Task<int> GetUnreadCountAsync(Guid userId, Guid bandChatId);

    /// <summary>
    /// Gets the last message from a specific band chat.
    /// </summary>
    /// <param name="bandChatId">The unique identifier of the band chat.</param>
    /// <returns>The last message in the chat if any, otherwise null.</returns>
    Task<BandMessage?> GetLastMessageForChatAsync(Guid bandChatId);

    /// <summary>
    /// Creates a new band message in the repository.
    /// </summary>
    /// <param name="bandMessage">The band message entity to create.</param>
    /// <returns>The created band message entity with generated values.</returns>
    Task<BandMessage> InsertAsync(BandMessage bandMessage);

    /// <summary>
    /// Updates an existing band message in the repository.
    /// </summary>
    /// <param name="bandMessage">The band message entity with updated values.</param>
    Task UpdateAsync(BandMessage bandMessage);

    /// <summary>
    /// Marks a specific message as read.
    /// </summary>
    /// <param name="messageId">The unique identifier of the message to mark as read.</param>
    Task MarkAsReadAsync(Guid messageId);

    /// <summary>
    /// Deletes a band message from the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the band message to delete.</param>
    Task DeleteAsync(Guid id);
}
