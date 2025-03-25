using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TuneSpace.Core.DTOs.Requests.Spotify;
using TuneSpace.Core.DTOs.Responses.Spotify;
using TuneSpace.Core.Exceptions;
using TuneSpace.Core.Interfaces.IClients;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Core.Services;

internal class SpotifyService(
    ISpotifyClient spotifyClient, 
    IConfiguration configuration) : ISpotifyService
{
    private const string SpotifyRedirectUri = "http://localhost:5053/api/Spotify/callback";

    string ISpotifyService.GetSpotifyLoginUrl()
    {
        var state = GenerateRandomString(16);
        const string scope = "user-read-private user-read-email user-top-read playlist-modify-private playlist-modify-public user-read-recently-played";

        var redirectUrl = $"https://accounts.spotify.com/authorize?" +
                          $"response_type=code" +
                          $"&client_id={configuration["SpotifyApi:ClientId"]}" +
                          $"&scope={scope}" +
                          $"&redirect_uri={SpotifyRedirectUri}" +
                          $"&state={state}";

        return redirectUrl;
    }

    async Task<SpotifyTokenResponse> ISpotifyService.ExchangeCodeForToken(string code)
    {
        var parameters = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string?>("grant_type", "authorization_code"),
            new KeyValuePair<string, string?>("code", code),
            new KeyValuePair<string, string?>("redirect_uri", SpotifyRedirectUri),
            new KeyValuePair<string, string?>("client_id", configuration["SpotifyApi:ClientId"]),
            new KeyValuePair<string, string?>("client_secret", configuration["SpotifyApi:ClientSecret"])
        });
            
        try
        {
            var response = await spotifyClient.GetToken(parameters);
            var content = await response.Content.ReadAsStringAsync();

            var values = content.Split('"').ToList();

            var accessToken = values[3];
            var refreshToken = values[13];
            
            return new SpotifyTokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("rip");
            throw;
        }
    }

    async Task<SpotifyProfileDTO> ISpotifyService.GetUserSpotifyProfile(string token)
    {
        try
        {
            var response = await spotifyClient.GetUserInfo(token);

            if (!response.IsSuccessStatusCode)
            {
                throw new SpotifyApiException("Error retrieving Spotify user data");
            }
            
            var user = JsonConvert.DeserializeObject<SpotifyApiProfileResponse>(await response.Content.ReadAsStringAsync());

            var profile = new SpotifyProfileDTO
            {
                Username = user.Display_Name,
                FollowerCount = user.Followers.Total,
                ProfilePicture = user.Images[1].Url,
                SpotifyPlan = user.Product
            };
            
            return profile ?? throw new JsonSerializationException();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    async Task<List<TopArtistDTO>> ISpotifyService.GetUserTopArtists(string token)
    {
        try
        {
            var response = await spotifyClient.GetUserTopArtists(token);

            if (!response.IsSuccessStatusCode)
            {
                throw new SpotifyApiException("Error retrieving Spotify user top artists");
            }
            
            var rootObject = JsonConvert.DeserializeObject<UserTopArtistsResponse>(await response.Content.ReadAsStringAsync());

            var artistDtos = rootObject?.Items.Select(item => 
                new TopArtistDTO(
                    item.Name,
                    item.Popularity,
                    item.Images.OrderByDescending(img => img.Width * img.Height).FirstOrDefault()?.Url
                )
            ).ToList();
            
            return artistDtos ?? throw new JsonSerializationException();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    async Task<List<RecentlyPlayedTrackDTO>> ISpotifyService.GetUserRecentlyPlayedTracks(string token)
    {
        try
        {
            var response = await spotifyClient.GetUserRecentlyPlayedTracks(token);

            if (!response.IsSuccessStatusCode)
            {
                throw new SpotifyApiException("Error retrieving Spotify user recently played tracks");
            }
            
            var rootObject = JsonConvert.DeserializeObject<UserRecentlyPlayedTracksResponse>(await response.Content.ReadAsStringAsync());

            var trackDtos = rootObject?.Items.Select(item => 
                new RecentlyPlayedTrackDTO
                {
                    TrackName = item.Track.Name,
                    ArtistName = string.Join(", ", item.Track.Artists.Select(artist => artist.Name)),
                    ArtistId = item.Track.Artists.First().Id,
                    AlbumName = item.Track.Album.Name,
                    AlbumImageUrl = item.Track.Album.Images.OrderByDescending(img => img.Width * img.Height).FirstOrDefault()?.Url,
                    PlayedAt = DateTime.Parse(item.Played_At)
                }
            ).ToList();
            
            return trackDtos ?? throw new JsonSerializationException();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    async Task<List<TopSongDTO>> ISpotifyService.GetUserTopSongs(string token)
    {
        try
        {
            var response = await spotifyClient.GetUserTopSongs(token);

            if (!response.IsSuccessStatusCode)
            {
                throw new SpotifyApiException("Error retrieving Spotify user top artists");
            }
            
            var rootObject = JsonConvert.DeserializeObject<UserTopSongsResponse>(await response.Content.ReadAsStringAsync());

            var songDtos = rootObject?.Items.Select(item => 
                new TopSongDTO(
                    item.Name,
                    item.Album.Artists.First().Name,
                    item.Album.Images.OrderByDescending(img => img.Width * img.Height).FirstOrDefault()?.Url
                )
            ).ToList();
            
            return songDtos ?? throw new JsonSerializationException();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    async Task<List<SearchSongDTO>> ISpotifyService.GetSongsBySearch(string token, string search)
    {
        try
        {
            var response = await spotifyClient.GetSongsBySearch(token, search);
            var rootObject = JsonConvert.DeserializeObject<SpotifySongSearchResponse>(await response.Content.ReadAsStringAsync());
            
            var songDtos = rootObject?.Tracks.Items.Select(item => 
                new SearchSongDTO(
                    item.Id,
                    item.Name,
                    item.Artists.First().Name,
                    item.Album.Images.OrderByDescending(img => img.Width * img.Height).FirstOrDefault()?.Url
                )
            ).ToList();
            
            return songDtos ?? throw new JsonSerializationException();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    async  Task<SpotifyArtistDTO> ISpotifyService.GetArtist(string token, string artistId)
    {
        try
        {
            var response = await spotifyClient.GetArtist(token, artistId);

            if (!response.IsSuccessStatusCode)
            {
                throw new SpotifyApiException("Error retrieving Spotify artist");
            }
            
            var artist = JsonConvert.DeserializeObject<SpotifyArtistDTO>(await response.Content.ReadAsStringAsync());
            
            return artist ?? throw new JsonSerializationException();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    async Task<List<SpotifyArtistDTO>> ISpotifyService.GetSeveralArtists(string token, string artistIds)
    {
        try
        {
            var response = await spotifyClient.GetSeveralArtists(token, artistIds);

            if (!response.IsSuccessStatusCode)
            {
                throw new SpotifyApiException("Error retrieving Spotify artists");
            }
            
            var artists = JsonConvert.DeserializeObject<SpotifySeveralArtistsResponse>(await response.Content.ReadAsStringAsync());
            
            return artists?.Artists ?? throw new JsonSerializationException();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async void CreatePlaylist(string token, CreatePlaylistRequest request)
    {
        var requestContent = ToLowercaseJsonStringContent(request);
        
        await spotifyClient.CreatePlaylist(token, request.UserId, requestContent);
    }
    
    private static StringContent ToLowercaseJsonStringContent(CreatePlaylistRequest request)
    {
        var dictionary = new Dictionary<string, object>
        {
            ["name"] = request.Name,
            ["description"] = request.Description,
            ["public"] = request.Public
        };
        
        var jsonString = JsonConvert.SerializeObject(dictionary);

        return new StringContent(jsonString, Encoding.UTF8, "application/json");
    }

    private static string GenerateRandomString(int length)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}