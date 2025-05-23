﻿using TuneSpace.Core.DTOs.Requests.Band;
using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Provides business logic services for managing bands in the TuneSpace application.
/// </summary>
public interface IBandService
{
    /// <summary>
    /// Creates a new band based on the provided request data.
    /// </summary>
    /// <param name="request">The data required to create a band, including name, description, genre, and image.</param>
    /// <returns>The created band entity if successful; otherwise, null.</returns>
    Task<Band?> CreateBand(CreateBandRequest request);

    /// <summary>
    /// Retrieves a band by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the band to retrieve.</param>
    /// <returns>The band if found; otherwise, null.</returns>
    Task<Band?> GetBandById(Guid id);

    /// <summary>
    /// Retrieves a band by its name.
    /// </summary>
    /// <param name="name">The name of the band to retrieve.</param>
    /// <returns>The band if found; otherwise, null.</returns>
    Task<Band?> GetBandByName(string name);

    /// <summary>
    /// Retrieves a band associated with a specific user.
    /// </summary>
    /// <param name="id">The unique identifier of the user who owns the band.</param>
    /// <returns>The band if found; otherwise, null.</returns>
    Task<Band?> GetBandByUserId(string id);

    /// <summary>
    /// Retrieves the image data for a specific band.
    /// </summary>
    /// <param name="bandId">The unique identifier of the band whose image should be retrieved.</param>
    /// <returns>The image data as a byte array if available; otherwise, null.</returns>
    Task<byte[]?> GetBandImage(Guid bandId);

    /// <summary>
    /// Updates an existing band with the provided information.
    /// </summary>
    /// <param name="request">The data containing updates to the band, which may include name, description, genre, and image.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateBand(UpdateBandRequest request);

    /// <summary>
    /// Deletes a band from the system.
    /// </summary>
    /// <param name="id">The unique identifier of the band to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteBand(Guid id);
}
