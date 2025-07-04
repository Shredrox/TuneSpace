using TuneSpace.Core.DTOs.Responses.Spotify;

namespace TuneSpace.Core.Interfaces.IServices;

public interface ISpotifyService
{
    /// <summary>
    /// Generates a Spotify login URL for user authentication
    /// </summary>
    /// <returns>Spotify login URL to redirect the user to</returns>
    string GetSpotifyLoginUrl();

    /// <summary>
    /// Generates a Spotify login URL with flow type support
    /// </summary>
    /// <param name="flowType">The type of OAuth flow ('login' or 'connect')</param>
    /// <returns>Spotify login URL to redirect the user to</returns>
    string GetSpotifyLoginUrl(string flowType);

    /// <summary>
    /// Exchanges an authorization code for a Spotify access token
    /// </summary>
    /// <param name="code">Authorization code received from Spotify callback</param>
    /// <returns>A token response containing access and refresh tokens</returns>
    Task<SpotifyTokenResponse> ExchangeCodeForTokenAsync(string code);

    /// <summary>
    /// Gets a user's Spotify profile information
    /// </summary>
    /// <param name="token">Spotify access token</param>
    /// <returns>User's Spotify profile details</returns>
    Task<SpotifyProfileDTO> GetUserSpotifyProfileAsync(string token);

    /// <summary>
    /// Gets a user's detailed Spotify profile information including email
    /// </summary>
    /// <param name="token">Spotify access token</param>
    /// <returns>User's detailed Spotify profile data</returns>
    Task<SpotifyApiProfileResponse> GetUserInfoAsync(string token);

    /// <summary>
    /// Gets a list of artists the user follows on Spotify
    /// </summary>
    /// <param name="token">Spotify access token</param>
    /// <returns>List of followed artists</returns>
    Task<List<SpotifyArtistDTO>> GetUserFollowedArtistsAsync(string token);

    /// <summary>
    /// Gets a user's top artists from Spotify
    /// </summary>
    /// <param name="token">Spotify access token</param>
    /// <returns>List of user's top artists</returns>
    Task<List<SpotifyArtistDTO>> GetUserTopArtistsAsync(string token);

    /// <summary>
    /// Gets a user's recently played tracks on Spotify
    /// </summary>
    /// <param name="token">Spotify access token</param>
    /// <returns>List of recently played tracks</returns>
    Task<List<RecentlyPlayedTrackDTO>> GetUserRecentlyPlayedTracksAsync(string token);


    /// <summary>
    /// Gets listening statistics for a specific time period (today or this week)
    /// </summary>
    /// <param name="token">Spotify access token</param>
    /// <param name="period">Time period to filter by ("today" or "this-week")</param>
    /// <returns>Listening statistics including tracks, total hours played, and other metrics</returns>
    Task<RecentlyPlayedStatsResponse> GetListeningStatsForPeriodAsync(string token, string period);

    /// <summary>
    /// Gets information for a specific artist from Spotify
    /// </summary>
    /// <param name="token">Spotify access token</param>
    /// <param name="artistId">Spotify artist ID</param>
    /// <returns>Artist information</returns>
    Task<SpotifyArtistDTO> GetArtistAsync(string token, string artistId);

    /// <summary>
    /// Gets information for multiple artists from Spotify
    /// </summary>
    /// <param name="token">Spotify access token</param>
    /// <param name="artistIds">Comma-separated list of Spotify artist IDs</param>
    /// <returns>List of artist information</returns>
    Task<List<SpotifyArtistDTO>> GetSeveralArtistsAsync(string token, string artistIds);

    /// <summary>
    /// Gets a user's top songs from Spotify
    /// </summary>
    /// <param name="token">Spotify access token</param>
    /// <returns>List of user's top songs</returns>
    Task<List<TopSongDTO>> GetUserTopSongsAsync(string token);

    /// <summary>
    /// Searches for songs on Spotify matching the given query
    /// </summary>
    /// <param name="token">Spotify access token</param>
    /// <param name="search">Search query</param>
    /// <returns>List of songs matching the search query</returns>
    Task<List<SearchSongDTO>> GetSongsBySearchAsync(string token, string search);

    /// <summary>
    /// Refreshes a Spotify access token using a refresh token
    /// </summary>
    /// <param name="refreshToken">Spotify refresh token</param>
    /// <returns>New access and refresh tokens</returns>
    Task<SpotifyTokenResponse> RefreshAccessTokenAsync(string refreshToken);

    /// <summary>
    /// Performs a search across Spotify's catalog for albums and/or artists
    /// </summary>
    /// <param name="token">Spotify access token</param>
    /// <param name="query">Search query</param>
    /// <param name="types">Comma-separated list of item types to search for (album,artist)</param>
    /// <param name="limit">Maximum number of results to return per type</param>
    /// <param name="offset">Offset for pagination, specifying the starting point of results</param>
    /// <returns>Search results matching the query with artist and/or album data</returns>
    Task<SpotifySearchResponse> SearchAsync(string token, string query, string types, int limit, int offset = 0);
}
