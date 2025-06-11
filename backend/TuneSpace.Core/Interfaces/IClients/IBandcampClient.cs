using TuneSpace.Core.Models;

namespace TuneSpace.Core.Interfaces.IClients;

/// <summary>
/// Defines the contract for interacting with the Bandcamp API to discover artists and music.
/// Provides methods for searching artists by various criteria including genre, location, and discovery patterns.
/// </summary>
public interface IBandcampClient
{
    /// <summary>
    /// Discovers artists from Bandcamp filtered by a specific genre.
    /// </summary>
    /// <param name="genre">The music genre to filter artists by (e.g., "rock", "jazz", "electronic")</param>
    /// <param name="limit">The maximum number of artists to return. Default is 50.</param>
    /// <returns>A list of <see cref="BandcampArtistModel"/> representing discovered artists</returns>
    Task<List<BandcampArtistModel>> DiscoverArtistsByGenreAsync(string genre, int limit = 50);

    /// <summary>
    /// Discovers artists from Bandcamp filtered by geographic location.
    /// </summary>
    /// <param name="location">The geographic location to filter artists by (e.g., "New York", "London", "Tokyo")</param>
    /// <param name="limit">The maximum number of artists to return. Default is 50.</param>
    /// <returns>A list of <see cref="BandcampArtistModel"/> representing discovered artists</returns>
    Task<List<BandcampArtistModel>> DiscoverArtistsByLocationAsync(string location, int limit = 50);

    /// <summary>
    /// Discovers artists from Bandcamp filtered by multiple genres with sorting options.
    /// </summary>
    /// <param name="genres">A list of music genres to filter artists by</param>
    /// <param name="sortBy">The sorting criteria for results. Default is "new". Common values: "new", "popular", "trending"</param>
    /// <param name="limit">The maximum number of artists to return. Default is 50.</param>
    /// <returns>A list of <see cref="BandcampArtistModel"/> representing discovered artists</returns>
    Task<List<BandcampArtistModel>> DiscoverArtistsByGenresAsync(List<string> genres, string sortBy = "new", int limit = 50);

    /// <summary>
    /// Discovers a random selection of artists from Bandcamp filtered by a specific genre.
    /// Provides serendipitous discovery by returning random artists within the specified genre.
    /// </summary>
    /// <param name="genre">The music genre to filter artists by</param>
    /// <param name="limit">The maximum number of artists to return. Default is 50.</param>
    /// <returns>A list of <see cref="BandcampArtistModel"/> representing randomly discovered artists</returns>
    Task<List<BandcampArtistModel>> DiscoverRandomArtistsByGenreAsync(string genre, int limit = 50);

    /// <summary>
    /// Discovers a random selection of artists from Bandcamp filtered by multiple genres.
    /// Provides serendipitous discovery across multiple musical styles.
    /// </summary>
    /// <param name="genres">A list of music genres to filter artists by</param>
    /// <param name="limit">The maximum number of artists to return. Default is 50.</param>
    /// <returns>A list of <see cref="BandcampArtistModel"/> representing randomly discovered artists</returns>
    Task<List<BandcampArtistModel>> DiscoverRandomArtistsByGenresAsync(List<string> genres, int limit = 50);

    /// <summary>
    /// Discovers artists from Bandcamp filtered by both genres and geographic location with sorting options.
    /// Combines genre and location-based filtering for more targeted discovery.
    /// </summary>
    /// <param name="genres">A list of music genres to filter artists by</param>
    /// <param name="location">The geographic location to filter artists by</param>
    /// <param name="sortBy">The sorting criteria for results. Default is "rand" for random ordering. Other values: "new", "popular"</param>
    /// <param name="limit">The maximum number of artists to return. Default is 50.</param>
    /// <returns>A list of <see cref="BandcampArtistModel"/> representing discovered artists</returns>
    Task<List<BandcampArtistModel>> DiscoverArtistsByGenresAndLocationAsync(List<string> genres, string location, string sortBy = "rand", int limit = 50);

    /// <summary>
    /// Retrieves the most recent music releases from Bandcamp.
    /// Provides access to newly published albums, EPs, and tracks across all genres.
    /// </summary>
    /// <param name="limit">The maximum number of recent releases to return. Default is 50.</param>
    /// <returns>A list of <see cref="BandcampArtistModel"/> representing artists with recent releases</returns>
    Task<List<BandcampArtistModel>> GetRecentReleasesAsync(int limit = 50);
}
