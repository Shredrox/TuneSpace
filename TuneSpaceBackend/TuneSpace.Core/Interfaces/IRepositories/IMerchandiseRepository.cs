using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IRepositories;

/// <summary>
/// Repository interface for handling merchandise data operations.
/// </summary>
public interface IMerchandiseRepository
{
    /// <summary>
    /// Creates a new merchandise item in the system.
    /// </summary>
    /// <param name="merchandise">The merchandise item to create.</param>
    /// <returns>The created merchandise item with assigned ID.</returns>
    Task<Merchandise> CreateMerchandiseAsync(Merchandise merchandise);

    /// <summary>
    /// Retrieves a merchandise item by its unique identifier.
    /// </summary>
    /// <param name="merchandiseId">The unique identifier of the merchandise item.</param>
    /// <returns>The merchandise item if found; otherwise, null.</returns>
    Task<Merchandise?> GetMerchandiseByIdAsync(Guid merchandiseId);

    /// <summary>
    /// Retrieves all merchandise items in the system.
    /// </summary>
    /// <returns>A collection of all merchandise items.</returns>
    Task<IEnumerable<Merchandise>> GetAllMerchandisesAsync();

    /// <summary>
    /// Retrieves all merchandise items associated with a specific band.
    /// </summary>
    /// <param name="bandId">The unique identifier of the band.</param>
    /// <returns>A collection of merchandise items for the specified band.</returns>
    Task<IEnumerable<Merchandise>> GetAllMerchandisesByBandIdAsync(Guid bandId);

    /// <summary>
    /// Updates an existing merchandise item.
    /// </summary>
    /// <param name="merchandise">The merchandise item with updated information.</param>
    /// <returns>The updated merchandise item.</returns>
    Task<Merchandise> UpdateMerchandiseAsync(Merchandise merchandise);

    /// <summary>
    /// Deletes a merchandise item from the system.
    /// </summary>
    /// <param name="merchandiseId">The unique identifier of the merchandise item to delete.</param>
    /// <returns>True if the merchandise was successfully deleted; otherwise, false.</returns>
    Task<bool> DeleteMerchandiseAsync(Guid merchandiseId);
}
