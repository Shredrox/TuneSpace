using System.Net.Http.Headers;
using TuneSpace.Core.Interfaces.IClients;

namespace TuneSpace.Infrastructure.Clients;

internal class SpotifyClient(HttpClient httpClient) : ISpotifyClient
{
    private readonly HttpClient _httpClient = httpClient;

    private const string BaseApiUrl = "https://api.spotify.com/v1";
    private const string AuthUrl = "https://accounts.spotify.com/api/token";

    async Task<HttpResponseMessage> ISpotifyClient.GetToken(FormUrlEncodedContent parameters)
    {
        return await _httpClient.PostAsync(AuthUrl, parameters);
    }

    async Task<HttpResponseMessage> ISpotifyClient.GetUserInfo(string token)
    {
        SetAuthorizationHeader(token);
        return await _httpClient.GetAsync($"{BaseApiUrl}/me");
    }

    async Task<HttpResponseMessage> ISpotifyClient.GetUserRecentlyPlayedTracks(string token)
    {
        SetAuthorizationHeader(token);
        return await _httpClient.GetAsync($"{BaseApiUrl}/me/player/recently-played");
    }

    async Task<HttpResponseMessage> ISpotifyClient.GetUserFollowedArtists(string token, string? after)
    {
        SetAuthorizationHeader(token);

        var url = $"{BaseApiUrl}/me/following?type=artist&limit=50";
        if (!string.IsNullOrEmpty(after))
        {
            url += $"&after={after}";
        }

        return await _httpClient.GetAsync(url);
    }

    async Task<HttpResponseMessage> ISpotifyClient.GetUserTopArtists(string token)
    {
        SetAuthorizationHeader(token);
        return await _httpClient.GetAsync($"{BaseApiUrl}/me/top/artists?time_range=short_term&limit=5&offset=0");
    }

    async Task<HttpResponseMessage> ISpotifyClient.GetUserTopSongs(string token)
    {
        SetAuthorizationHeader(token);
        return await _httpClient.GetAsync($"{BaseApiUrl}/me/top/tracks?time_range=short_term&limit=5&offset=0");
    }

    async Task<HttpResponseMessage> ISpotifyClient.GetSongsBySearch(string token, string search)
    {
        SetAuthorizationHeader(token);
        return await _httpClient.GetAsync($"{BaseApiUrl}/search?q={Uri.EscapeDataString(search)}&type=track&limit=5");
    }

    async Task<HttpResponseMessage> ISpotifyClient.CreatePlaylist(string token, string userId, StringContent requestContent)
    {
        SetAuthorizationHeader(token);
        return await _httpClient.PostAsync($"{BaseApiUrl}/users/{userId}/playlists", requestContent);
    }

    async Task<HttpResponseMessage> ISpotifyClient.GetArtist(string token, string artistId)
    {
        SetAuthorizationHeader(token);
        return await _httpClient.GetAsync($"{BaseApiUrl}/artists/{artistId}");
    }

    async Task<HttpResponseMessage> ISpotifyClient.GetSeveralArtists(string token, string artistIds)
    {
        SetAuthorizationHeader(token);
        return await _httpClient.GetAsync($"{BaseApiUrl}/artists?ids={artistIds}");
    }

    async Task<HttpResponseMessage> ISpotifyClient.RefreshAccessToken(FormUrlEncodedContent parameters)
    {
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        return await _httpClient.PostAsync(AuthUrl, parameters);
    }

    async Task<HttpResponseMessage> ISpotifyClient.Search(string token, string query, string types, int limit)
    {
        SetAuthorizationHeader(token);

        var requestUrl = $"{BaseApiUrl}/search?q={Uri.EscapeDataString(query)}&type={types}";

        if (limit > 0)
        {
            requestUrl += $"&limit={limit}";
        }

        // if (offset > 0)
        // {
        //     requestUrl += $"&offset={offset}";
        // }

        // if (!string.IsNullOrEmpty(market))
        // {
        //     requestUrl += $"&market={market}";
        // }

        return await _httpClient.GetAsync(requestUrl);
    }

    private void SetAuthorizationHeader(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
