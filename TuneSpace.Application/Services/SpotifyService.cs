using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TuneSpace.Application.Common;
using TuneSpace.Core.DTOs.Requests.Spotify;
using TuneSpace.Core.DTOs.Responses.Spotify;
using TuneSpace.Core.Exceptions;
using TuneSpace.Core.Interfaces.IClients;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Application.Services;

internal class SpotifyService(
    ISpotifyClient spotifyClient,
    IConfiguration configuration,
    ILogger<SpotifyService> logger) : ISpotifyService
{
    private readonly ISpotifyClient _spotifyClient = spotifyClient;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<SpotifyService> _logger = logger;

    private const string SpotifyRedirectUri = "http://localhost:5053/api/Spotify/callback";

    string ISpotifyService.GetSpotifyLoginUrl()
    {
        var state = Helpers.GenerateRandomString(16);
        const string scope = "user-read-private user-read-email user-top-read playlist-modify-private playlist-modify-public user-read-recently-played user-follow-read";

        var redirectUrl = $"https://accounts.spotify.com/authorize?" +
                          $"response_type=code" +
                          $"&client_id={_configuration["SpotifyApi:ClientId"]}" +
                          $"&scope={scope}" +
                          $"&redirect_uri={SpotifyRedirectUri}" +
                          $"&state={state}";

        return redirectUrl;
    }

    async Task<SpotifyTokenResponse> ISpotifyService.ExchangeCodeForToken(string code)
    {
        var parameters = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string?>("grant_type", "authorization_code"),
            new KeyValuePair<string, string?>("code", code),
            new KeyValuePair<string, string?>("redirect_uri", SpotifyRedirectUri),
            new KeyValuePair<string, string?>("client_id", _configuration["SpotifyApi:ClientId"]),
            new KeyValuePair<string, string?>("client_secret", _configuration["SpotifyApi:ClientSecret"])
        ]);

        try
        {
            var response = await _spotifyClient.GetToken(parameters);
            var content = await response.Content.ReadAsStringAsync();

            var tokenResponse = JsonConvert.DeserializeObject<SpotifyTokenResponse>(content,
                new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    }
                }) ?? throw new SpotifyApiException("Failed to deserialize token response");

            return tokenResponse;
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(e, "Error exchanging code for token");
            throw;
        }
    }

    async Task<SpotifyProfileDTO> ISpotifyService.GetUserSpotifyProfile(string token)
    {
        try
        {
            var response = await _spotifyClient.GetUserInfo(token);

            if (!response.IsSuccessStatusCode)
            {
                throw new SpotifyApiException("Error retrieving Spotify user data");
            }

            var user = JsonConvert.DeserializeObject<SpotifyApiProfileResponse>(await response.Content.ReadAsStringAsync())
            ?? throw new SpotifyApiException("Failed to deserialize Spotify profile response");

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
            _logger.LogError(ex, "Error retrieving Spotify user profile");
            throw;
        }
    }

    async Task<List<TopArtistDTO>> ISpotifyService.GetUserTopArtists(string token)
    {
        try
        {
            var response = await _spotifyClient.GetUserTopArtists(token);

            if (!response.IsSuccessStatusCode)
            {
                throw new SpotifyApiException("Error retrieving Spotify user top artists");
            }

            var rootObject = JsonConvert.DeserializeObject<UserTopArtistsResponse>(await response.Content.ReadAsStringAsync());

            var artistDtos = rootObject?.Items.Select(item =>
                new TopArtistDTO(
                    item.Name,
                    item.Popularity,
                    item.Images.OrderByDescending(img => img.Width * img.Height).FirstOrDefault()?.Url ?? string.Empty
                )
            ).ToList();

            return artistDtos ?? throw new JsonSerializationException();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Spotify user top artists");
            throw;
        }
    }

    async Task<List<SpotifyArtistDTO>> ISpotifyService.GetUserFollowedArtists(string token)
    {
        try
        {
            var allArtists = new List<SpotifyArtistDTO>();
            string? afterCursor = null;
            bool hasMoreArtists = true;

            while (hasMoreArtists)
            {
                var response = await _spotifyClient.GetUserFollowedArtists(token, afterCursor);

                if (!response.IsSuccessStatusCode)
                {
                    throw new SpotifyApiException("Error retrieving Spotify user followed artists");
                }

                var rootObject = JsonConvert.DeserializeObject<UserFollowedArtistsResponse>(await response.Content.ReadAsStringAsync());

                if (rootObject?.Artists?.Items == null)
                {
                    throw new JsonSerializationException("Failed to deserialize artists response");
                }

                var artistDtos = rootObject.Artists.Items.Select(item =>
                    new SpotifyArtistDTO
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Popularity = item.Popularity,
                        Images = item.Images.OrderByDescending(img => img.Width * img.Height).ToList(),
                        Genres = item.Genres,
                        Followers = item.Followers
                    }
                ).ToList();

                allArtists.AddRange(artistDtos);

                hasMoreArtists = !string.IsNullOrEmpty(rootObject.Artists.Cursors?.After);
                afterCursor = rootObject.Artists.Cursors?.After;
            }

            return allArtists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Spotify user followed artists");
            throw;
        }
    }

    async Task<List<RecentlyPlayedTrackDTO>> ISpotifyService.GetUserRecentlyPlayedTracks(string token)
    {
        try
        {
            var response = await _spotifyClient.GetUserRecentlyPlayedTracks(token);

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
                    AlbumImageUrl = item.Track.Album.Images.OrderByDescending(img => img.Width * img.Height).FirstOrDefault()?.Url ?? string.Empty,
                    PlayedAt = DateTime.Parse(item.Played_At)
                }
            ).ToList();

            return trackDtos ?? throw new JsonSerializationException();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Spotify user recently played tracks");
            throw;
        }
    }

    async Task<List<TopSongDTO>> ISpotifyService.GetUserTopSongs(string token)
    {
        try
        {
            var response = await _spotifyClient.GetUserTopSongs(token);

            if (!response.IsSuccessStatusCode)
            {
                throw new SpotifyApiException("Error retrieving Spotify user top artists");
            }

            var rootObject = JsonConvert.DeserializeObject<UserTopSongsResponse>(await response.Content.ReadAsStringAsync());

            var songDtos = rootObject?.Items.Select(item =>
                new TopSongDTO(
                    item.Name,
                    item.Album.Artists.First().Name,
                    item.Album.Images.OrderByDescending(img => img.Width * img.Height).FirstOrDefault()?.Url ?? string.Empty
                )
            ).ToList();

            return songDtos ?? throw new JsonSerializationException();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Spotify user top songs");
            throw;
        }
    }

    async Task<List<SearchSongDTO>> ISpotifyService.GetSongsBySearch(string token, string search)
    {
        try
        {
            var response = await _spotifyClient.GetSongsBySearch(token, search);
            var rootObject = JsonConvert.DeserializeObject<SpotifySongSearchResponse>(await response.Content.ReadAsStringAsync());

            var songDtos = rootObject?.Tracks.Items.Select(item =>
                new SearchSongDTO(
                    item.Id,
                    item.Name,
                    item.Artists.First().Name,
                    item.Album.Images.OrderByDescending(img => img.Width * img.Height).FirstOrDefault()?.Url ?? string.Empty
                )
            ).ToList();

            return songDtos ?? throw new JsonSerializationException();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Spotify songs by search");
            throw;
        }
    }

    async Task<SpotifyArtistDTO> ISpotifyService.GetArtist(string token, string artistId)
    {
        try
        {
            var response = await _spotifyClient.GetArtist(token, artistId);

            if (!response.IsSuccessStatusCode)
            {
                throw new SpotifyApiException("Error retrieving Spotify artist");
            }

            var artist = JsonConvert.DeserializeObject<SpotifyArtistDTO>(await response.Content.ReadAsStringAsync());

            return artist ?? throw new JsonSerializationException();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Spotify artist");
            throw;
        }
    }

    async Task<List<SpotifyArtistDTO>> ISpotifyService.GetSeveralArtists(string token, string artistIds)
    {
        try
        {
            var response = await _spotifyClient.GetSeveralArtists(token, artistIds);

            if (!response.IsSuccessStatusCode)
            {
                throw new SpotifyApiException("Error retrieving Spotify artists");
            }

            var artists = JsonConvert.DeserializeObject<SpotifySeveralArtistsResponse>(await response.Content.ReadAsStringAsync());

            return artists?.Artists ?? throw new JsonSerializationException();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Spotify artists");
            throw;
        }
    }

    async Task ISpotifyService.CreatePlaylist(string token, CreatePlaylistRequest request)
    {
        var requestContent = Helpers.ToLowercaseJsonStringContent(request);

        try
        {
            await _spotifyClient.CreatePlaylist(token, request.UserId, requestContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Spotify playlist");
            throw;
        }
    }

    async Task<SpotifyTokenResponse> ISpotifyService.RefreshAccessToken(string refreshToken)
    {
        var clientId = _configuration["SpotifyApi:ClientId"] ??
            throw new ArgumentNullException(nameof(_configuration), "Spotify client ID is not configured");
        var clientSecret = _configuration["SpotifyApi:ClientSecret"] ??
            throw new ArgumentNullException(nameof(_configuration), "Spotify client secret is not configured");

        var parameters = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken },
            { "client_id", clientId },
            { "client_secret", clientSecret }
        };

        var content = new FormUrlEncodedContent(parameters);

        try
        {
            var response = await _spotifyClient.RefreshAccessToken(content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new SpotifyApiException($"Failed to refresh Spotify token: {response.StatusCode}, {errorContent}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<SpotifyTokenResponse>(responseString,
                new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    }
                }) ?? throw new SpotifyApiException("Failed to deserialize Spotify token response");

            if (string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                tokenResponse.RefreshToken = refreshToken;
            }

            return tokenResponse;
        }
        catch (Exception ex) when (ex is not SpotifyApiException)
        {
            _logger.LogError(ex, "Error refreshing Spotify token");
            throw new SpotifyApiException($"Error refreshing Spotify token: {ex.Message}");
        }
    }

    async Task<SpotifySearchResponse> ISpotifyService.SearchAsync(string token, string query, string types, int limit)
    {
        try
        {
            var response = await _spotifyClient.Search(token, query, types, limit);

            if (!response.IsSuccessStatusCode)
            {
                throw new SpotifyApiException($"Failed to search Spotify: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SpotifySearchResponse>(content) ??
                   throw new JsonSerializationException("Failed to deserialize Spotify search response");
        }
        catch (Exception ex) when (ex is not SpotifyApiException)
        {
            _logger.LogError(ex, "Error searching Spotify");
            throw new SpotifyApiException($"Error searching Spotify: {ex.Message}");
        }
    }
}
