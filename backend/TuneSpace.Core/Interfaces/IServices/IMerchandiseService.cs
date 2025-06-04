using TuneSpace.Core.DTOs.Requests.Merchandise;
using TuneSpace.Core.DTOs.Responses.Merchandise;
using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Provides business logic services for managing merchandise in the TuneSpace application.
/// </summary>
public interface IMerchandiseService
{
    /// <summary>
    /// Retrieves a merchandise item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the merchandise to retrieve.</param>
    /// <returns>The merchandise if found; otherwise, null.</returns>
    Task<MerchandiseResponse?> GetMerchandiseByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all merchandise items for a specific band.
    /// </summary>
    /// <param name="bandId">The unique identifier of the band.</param>
    /// <returns>A list of merchandise items for the specified band.</returns>
    Task<IEnumerable<MerchandiseResponse>> GetMerchandiseByBandIdAsync(Guid bandId);

    /// <summary>
    /// Creates a new merchandise item based on the provided request data.
    /// </summary>
    /// <param name="request">The data required to create a merchandise item.</param>
    /// <returns>The created merchandise entity if successful; otherwise, null.</returns>
    Task<Merchandise?> CreateMerchandiseAsync(CreateMerchandiseRequest request);

    /// <summary>
    /// Updates an existing merchandise item with the provided information.
    /// </summary>
    /// <param name="request">The data containing updates to the merchandise item.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateMerchandiseAsync(UpdateMerchandiseRequest request);

    /// <summary>
    /// Deletes a merchandise item from the system.
    /// </summary>
    /// <param name="id">The unique identifier of the merchandise to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteMerchandiseAsync(Guid id);
}
