using TuneSpace.Core.DTOs.Requests.Spotify;
using TuneSpace.Core.DTOs.Responses.Spotify;

namespace TuneSpace.Core.Interfaces.IServices;

public interface ISpotifyService
{
    string GetSpotifyLoginUrl();
    Task<string> ExchangeCodeForToken(string code);
    Task<SpotifyProfileDTO> GetUserSpotifyProfile(string token);
    Task<List<TopArtistDTO>> GetUserTopArtists(string token);
    Task<SpotifyArtistDTO> GetArtist(string token, string artistId);
    Task<List<TopSongDTO>> GetUserTopSongs(string token);
    Task<List<SearchSongDTO>> GetSongsBySearch(string token, string search);
    void CreatePlaylist(string token, CreatePlaylistRequest request);
}