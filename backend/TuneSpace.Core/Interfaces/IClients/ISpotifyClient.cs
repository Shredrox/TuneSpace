namespace TuneSpace.Core.Interfaces.IClients;

/// <summary>
/// Provides methods for interacting with the Spotify Web API.
/// </summary>
public interface ISpotifyClient
{
    /// <summary>
    /// Requests an access token from Spotify's authorization server.
    /// </summary>
    /// <param name="parameters">The form parameters containing client credentials or authorization code.</param>
    /// <returns>HTTP response containing the access token response.</returns>
    Task<HttpResponseMessage> GetToken(FormUrlEncodedContent parameters);

    /// <summary>
    /// Retrieves the current user's profile information.
    /// </summary>
    /// <param name="token">The access token for authentication.</param>
    /// <returns>HTTP response containing user profile data.</returns>
    Task<HttpResponseMessage> GetUserInfo(string token);

    /// <summary>
    /// Retrieves the current user's top artists.
    /// </summary>
    /// <param name="token">The access token for authentication.</param>
    /// <returns>HTTP response containing the user's top artists.</returns>
    Task<HttpResponseMessage> GetUserTopArtists(string token);

    /// <summary>
    /// Retrieves the artists that the current user follows.
    /// </summary>
    /// <param name="token">The access token for authentication.</param>
    /// <param name="after">Optional cursor for pagination, specifying the last artist ID from previous request.</param>
    /// <returns>HTTP response containing the user's followed artists.</returns>
    Task<HttpResponseMessage> GetUserFollowedArtists(string token, string? after);

    /// <summary>
    /// Retrieves the tracks that the current user has recently played with time filtering.
    /// </summary>
    /// <param name="token">The access token for authentication.</param>
    /// <param name="after">Unix timestamp in milliseconds. Returns tracks played after this time.</param>
    /// <param name="before">Unix timestamp in milliseconds. Returns tracks played before this time.</param>
    /// <param name="limit">Maximum number of tracks to return (1-50, default 20).</param>
    /// <returns>HTTP response containing the user's recently played tracks within the specified time range.</returns>
    Task<HttpResponseMessage> GetUserRecentlyPlayedTracks(string token, long? after, long? before, int limit);

    /// <summary>
    /// Retrieves the current user's top tracks.
    /// </summary>
    /// <param name="token">The access token for authentication.</param>
    /// <returns>HTTP response containing the user's top songs.</returns>
    Task<HttpResponseMessage> GetUserTopSongs(string token);

    /// <summary>
    /// Retrieves information about a specific artist.
    /// </summary>
    /// <param name="token">The access token for authentication.</param>
    /// <param name="artistId">The Spotify ID of the artist.</param>
    /// <returns>HTTP response containing detailed information about the requested artist.</returns>
    Task<HttpResponseMessage> GetArtist(string token, string artistId);

    /// <summary>
    /// Retrieves information about multiple artists in a single request.
    /// </summary>
    /// <param name="token">The access token for authentication.</param>
    /// <param name="artistIds">Comma-separated list of the Spotify IDs for the artists.</param>
    /// <returns>HTTP response containing information about the requested artists.</returns>
    Task<HttpResponseMessage> GetSeveralArtists(string token, string artistIds);

    /// <summary>
    /// Searches for songs matching the provided search term.
    /// </summary>
    /// <param name="token">The access token for authentication.</param>
    /// <param name="search">The search query for finding songs.</param>
    /// <returns>HTTP response containing songs matching the search criteria.</returns>
    Task<HttpResponseMessage> GetSongsBySearch(string token, string search);

    /// <summary>
    /// Performs a search across multiple Spotify item types.
    /// </summary>
    /// <param name="token">The access token for authentication.</param>
    /// <param name="query">The search query string.</param>
    /// <param name="types">Comma-separated list of item types to search for (e.g., "artist,track").</param>
    /// <param name="limit">Maximum number of results to return per type.</param>
    /// <returns>HTTP response containing search results across specified types.</returns>
    Task<HttpResponseMessage> Search(string token, string query, string types, int limit);

    /// <summary>
    /// Creates a new playlist for the specified user.
    /// </summary>
    /// <param name="token">The access token for authentication.</param>
    /// <param name="userId">The Spotify ID of the user to create the playlist for.</param>
    /// <param name="requestContent">JSON content containing playlist details (name, description, public status).</param>
    /// <returns>HTTP response containing the created playlist information.</returns>
    Task<HttpResponseMessage> CreatePlaylist(string token, string userId, StringContent requestContent);

    /// <summary>
    /// Refreshes an expired access token using a refresh token.
    /// </summary>
    /// <param name="parameters">The form parameters containing refresh token and client credentials.</param>
    /// <returns>HTTP response containing the refreshed access token.</returns>
    Task<HttpResponseMessage> RefreshAccessToken(FormUrlEncodedContent parameters);
}
