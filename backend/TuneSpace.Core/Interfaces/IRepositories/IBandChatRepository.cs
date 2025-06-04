using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IRepositories;

/// <summary>
/// Repository interface for managing band chat operations in the data layer.
/// </summary>
public interface IBandChatRepository
{
    /// <summary>
    /// Retrieves a band chat by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the band chat.</param>
    /// <returns>The band chat if found, otherwise null.</returns>
    Task<BandChat?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves a specific band chat between a user and a band.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="bandId">The unique identifier of the band.</param>
    /// <returns>The band chat if found, otherwise null.</returns>
    Task<BandChat?> GetBandChatAsync(Guid userId, Guid bandId);

    /// <summary>
    /// Retrieves all band chats for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A collection of band chats associated with the user.</returns>
    Task<IEnumerable<BandChat>> GetUserBandChatsAsync(Guid userId);

    /// <summary>
    /// Retrieves all chats for a specific band.
    /// </summary>
    /// <param name="bandId">The unique identifier of the band.</param>
    /// <returns>A collection of chats associated with the band.</returns>
    Task<IEnumerable<BandChat>> GetBandChatsAsync(Guid bandId);

    /// <summary>
    /// Creates a new band chat in the repository.
    /// </summary>
    /// <param name="bandChat">The band chat entity to create.</param>
    /// <returns>The created band chat entity with generated values.</returns>
    Task<BandChat> InsertAsync(BandChat bandChat);

    /// <summary>
    /// Updates an existing band chat in the repository.
    /// </summary>
    /// <param name="bandChat">The band chat entity with updated values.</param>
    Task UpdateAsync(BandChat bandChat);

    /// <summary>
    /// Deletes a band chat from the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the band chat to delete.</param>
    Task DeleteAsync(Guid id);
}
