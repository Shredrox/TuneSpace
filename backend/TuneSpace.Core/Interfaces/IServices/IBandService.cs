using TuneSpace.Core.DTOs.Requests.Band;
using TuneSpace.Core.DTOs.Responses.Band;
using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Provides business logic services for managing bands in the TuneSpace application.
/// </summary>
public interface IBandService
{
    /// <summary>
    /// Retrieves a band by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the band to retrieve.</param>
    /// <returns>The band if found; otherwise, null.</returns>
    Task<Band?> GetBandByIdAsync(Guid id);

    /// <summary>
    /// Retrieves a band by its name.
    /// </summary>
    /// <param name="name">The name of the band to retrieve.</param>
    /// <returns>The band if found; otherwise, null.</returns>
    Task<Band?> GetBandByNameAsync(string name);

    /// <summary>
    /// Retrieves a band associated with a specific user.
    /// </summary>
    /// <param name="id">The unique identifier of the user who owns the band.</param>
    /// <returns>The band if found; otherwise, null.</returns>
    Task<BandResponse?> GetBandByUserIdAsync(string id);

    /// <summary>
    /// Retrieves the image data for a specific band.
    /// </summary>
    /// <param name="bandId">The unique identifier of the band whose image should be retrieved.</param>
    /// <returns>The image data as a byte array if available; otherwise, null.</returns>
    Task<byte[]?> GetBandImageAsync(Guid bandId);

    /// <summary>
    /// Creates a new band based on the provided request data.
    /// </summary>
    /// <param name="request">The data required to create a band, including name, description, genre, and image.</param>
    /// <returns>The created band entity if successful; otherwise, null.</returns>
    Task<Band?> CreateBandAsync(CreateBandRequest request);

    /// <summary>
    /// Updates an existing band with the provided information.
    /// </summary>
    /// <param name="request">The data containing updates to the band, which may include name, description, genre, and image.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateBandAsync(UpdateBandRequest request);

    /// <summary>
    /// Adds a user as a member to a specific band.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to add as a member.</param>
    /// <param name="bandId">The unique identifier of the band to which the user will be added.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddMemberToBandAsync(Guid userId, Guid bandId);

    /// <summary>
    /// Deletes a band from the system.
    /// </summary>
    /// <param name="id">The unique identifier of the band to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteBandAsync(Guid id);
}
