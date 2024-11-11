using System.Net.Http.Headers;
using TuneSpace.Core.Interfaces.IClients;

namespace TuneSpace.Infrastructure.Clients;

public class SpotifyClient(HttpClient httpClient) : ISpotifyClient
{
    public async Task<HttpResponseMessage> GetToken(FormUrlEncodedContent parameters)
    {
        const string tokenEndpoint = "https://accounts.spotify.com/api/token";
        
        return await httpClient.PostAsync(tokenEndpoint, parameters);
    }
    
    public async Task<HttpResponseMessage> GetUserInfo(string token)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await httpClient.GetAsync("https://api.spotify.com/v1/me");
    }

    public async Task<HttpResponseMessage> GetUserTopArtists(string token)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await httpClient.GetAsync("https://api.spotify.com/v1/me/top/artists?time_range=short_term&limit=5&offset=0");
    }
    
    public async Task<HttpResponseMessage> GetUserTopSongs(string token)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await httpClient.GetAsync("https://api.spotify.com/v1/me/top/tracks?time_range=short_term&limit=5&offset=0");
    }
    
    public async Task<HttpResponseMessage> GetSongsBySearch(string token, string search)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        return await httpClient.GetAsync($"https://api.spotify.com/v1/search?q={search}&type=track&limit=5");
    }
    
    public async Task<HttpResponseMessage> CreatePlaylist(string token, string userId, StringContent requestContent)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        return await httpClient.PostAsync($"https://api.spotify.com/v1/users/{userId}/playlists", requestContent);
    }
}