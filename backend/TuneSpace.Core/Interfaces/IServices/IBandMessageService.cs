using TuneSpace.Core.DTOs.Responses.BandChat;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Service interface for managing band message business logic and operations.
/// </summary>
public interface IBandMessageService
{
    /// <summary>
    /// Retrieves messages from a specific band chat with pagination support.
    /// </summary>
    /// <param name="bandChatId">The unique identifier of the band chat.</param>
    /// <param name="skip">The number of messages to skip (default: 0).</param>
    /// <param name="take">The number of messages to retrieve (default: 50).</param>
    /// <returns>A collection of band messages from the specified chat.</returns>
    Task<IEnumerable<BandMessageResponse>> GetChatMessagesAsync(Guid bandChatId, int skip, int take);

    /// <summary>
    /// Gets the count of unread messages for a user in a specific band chat.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="bandChatId">The unique identifier of the band chat.</param>
    /// <returns>The number of unread messages.</returns>
    Task<int> GetUnreadCountAsync(Guid userId, Guid bandChatId);

    /// <summary>
    /// Sends a message from a user to a band, creating or retrieving the chat as needed.
    /// </summary>
    /// <param name="userId">The unique identifier of the user sending the message.</param>
    /// <param name="bandId">The unique identifier of the band receiving the message.</param>
    /// <param name="content">The message content.</param>
    /// <returns>The created band message.</returns>
    Task<BandMessageResponse> SendMessageToBandAsync(Guid userId, Guid bandId, string content);

    /// <summary>
    /// Sends a message from a band to a user through a band member.
    /// </summary>
    /// <param name="bandId">The unique identifier of the band sending the message.</param>
    /// <param name="userId">The unique identifier of the user receiving the message.</param>
    /// <param name="content">The message content.</param>
    /// <param name="bandMemberId">The unique identifier of the band member sending on behalf of the band.</param>
    /// <returns>The created band message.</returns>
    Task<BandMessageResponse> SendMessageFromBandAsync(Guid bandId, Guid userId, string content, Guid bandMemberId);

    /// <summary>
    /// Marks a specific message as read with authorization checks.
    /// </summary>
    /// <param name="messageId">The unique identifier of the message to mark as read.</param>
    /// <param name="requestingUserId">The unique identifier of the user requesting the action.</param>
    Task MarkMessageAsReadAsync(Guid messageId, Guid requestingUserId);

    /// <summary>
    /// Deletes a message with authorization checks.
    /// </summary>
    /// <param name="messageId">The unique identifier of the message to delete.</param>
    /// <param name="requestingUserId">The unique identifier of the user requesting the deletion.</param>
    Task DeleteMessageAsync(Guid messageId, Guid requestingUserId);
}
