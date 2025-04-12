using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IRepositories;

/// <summary>
/// Provides data access methods for managing bands in the TuneSpace application.
/// </summary>
public interface IBandRepository
{
    /// <summary>
    /// Inserts a new band into the database.
    /// </summary>
    /// <param name="band">The band entity to insert.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InsertBand(Band band);

    /// <summary>
    /// Retrieves all bands from the database.
    /// </summary>
    /// <returns>A list of all band entities.</returns>
    Task<List<Band>> GetAllBands();

    /// <summary>
    /// Retrieves bands filtered by a specific genre.
    /// </summary>
    /// <param name="genre">The genre to filter by.</param>
    /// <returns>A list of bands matching the specified genre.</returns>
    Task<List<Band>> GetBandsByGenre(string genre);

    /// <summary>
    /// Retrieves bands filtered by a specific location.
    /// </summary>
    /// <param name="location">The location (city, country) to filter by.</param>
    /// <returns>A list of bands from the specified location.</returns>
    Task<List<Band>> GetBandsByLocation(string location);

    /// <summary>
    /// Retrieves bands filtered by both genre and location.
    /// </summary>
    /// <param name="genre">The genre to filter by.</param>
    /// <param name="location">The location to filter by.</param>
    /// <returns>A list of bands matching both the specified genre and location.</returns>
    Task<List<Band>> GetBandsByGenreAndLocation(string genre, string location);

    /// <summary>
    /// Retrieves a specific band by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the band.</param>
    /// <returns>The band if found; otherwise, null.</returns>
    Task<Band?> GetBandById(Guid id);

    /// <summary>
    /// Retrieves a band by its name.
    /// </summary>
    /// <param name="name">The name of the band to search for.</param>
    /// <returns>The band if found; otherwise, null.</returns>
    Task<Band?> GetBandByName(string name);

    /// <summary>
    /// Retrieves a band associated with a specific user.
    /// </summary>
    /// <param name="id">The user ID to search for.</param>
    /// <returns>The band if found; otherwise, null.</returns>
    Task<Band?> GetBandByUserId(string id);

    /// <summary>
    /// Updates an existing band's information in the database.
    /// </summary>
    /// <param name="band">The band entity with updated information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateBand(Band band);

    /// <summary>
    /// Deletes a band from the database.
    /// </summary>
    /// <param name="id">The unique identifier of the band to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteBand(Guid id);
}
