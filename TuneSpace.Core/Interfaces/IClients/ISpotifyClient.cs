namespace TuneSpace.Core.Interfaces.IClients;

public interface ISpotifyClient
{
    Task<HttpResponseMessage> GetToken(FormUrlEncodedContent parameters);
    Task<HttpResponseMessage> GetUserInfo(string token);
    Task<HttpResponseMessage> GetUserTopArtists(string token);
    Task<HttpResponseMessage> GetUserFollowedArtists(string token, string? after);
    Task<HttpResponseMessage> GetUserRecentlyPlayedTracks(string token);
    Task<HttpResponseMessage> GetUserTopSongs(string token);
    Task<HttpResponseMessage> GetArtist(string token, string artistId);
    Task<HttpResponseMessage> GetSeveralArtists(string token, string artistIds);
    Task<HttpResponseMessage> GetSongsBySearch(string token, string search);
    Task<HttpResponseMessage> Search(string token, string query, string types, int limit);
    Task<HttpResponseMessage> CreatePlaylist(string token, string userId, StringContent requestContent);
    Task<HttpResponseMessage> RefreshAccessToken(FormUrlEncodedContent parameters);
}