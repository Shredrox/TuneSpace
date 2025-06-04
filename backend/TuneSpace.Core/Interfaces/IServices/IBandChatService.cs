using TuneSpace.Core.DTOs.Responses.BandChat;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Service interface for managing band chat business logic and operations.
/// </summary>
public interface IBandChatService
{
    /// <summary>
    /// Creates a new band chat or retrieves an existing one between a user and a band.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="bandId">The unique identifier of the band.</param>
    /// <returns>The existing or newly created band chat.</returns>
    Task<BandChatResponse> CreateOrGetBandChatAsync(Guid userId, Guid bandId);

    /// <summary>
    /// Retrieves all band chats for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A collection of band chats associated with the user.</returns>
    Task<IEnumerable<BandChatResponse>> GetUserBandChatsAsync(Guid userId);

    /// <summary>
    /// Retrieves all chats for a specific band.
    /// </summary>
    /// <param name="bandId">The unique identifier of the band.</param>
    /// <returns>A collection of chats associated with the band.</returns>
    Task<IEnumerable<BandChatResponse>> GetBandChatsAsync(Guid bandId);

    /// <summary>
    /// Retrieves a specific band chat between a user and a band.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="bandId">The unique identifier of the band.</param>
    /// <returns>The band chat if found, otherwise null.</returns>
    Task<BandChatResponse?> GetBandChatAsync(Guid userId, Guid bandId);

    /// <summary>
    /// Retrieves a band chat by its unique identifier.
    /// </summary>
    /// <param name="chatId">The unique identifier of the band chat.</param>
    /// <returns>The band chat if found, otherwise null.</returns>
    Task<BandChatResponse?> GetBandChatByIdAsync(Guid chatId);

    /// <summary>
    /// Deletes a band chat with authorization checks.
    /// </summary>
    /// <param name="bandChatId">The unique identifier of the band chat to delete.</param>
    /// <param name="requestingUserId">The unique identifier of the user requesting the deletion.</param>
    Task DeleteBandChatAsync(Guid bandChatId, Guid requestingUserId);
}
