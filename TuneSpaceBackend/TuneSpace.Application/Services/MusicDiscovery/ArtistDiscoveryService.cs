using Microsoft.Extensions.Logging;
using TuneSpace.Core.DTOs.Responses.Spotify;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IClients;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;
using TuneSpace.Core.Models;
using TuneSpace.Core.Common;
using TuneSpace.Core.Interfaces.IInfrastructure;

namespace TuneSpace.Application.Services.MusicDiscovery;

internal class ArtistDiscoveryService(
    ISpotifyService spotifyService,
    ILastFmClient lastFmClient,
    IBandCachingService cachingService,
    IBandRepository bandRepository,
    ILogger<ArtistDiscoveryService> logger,
    IApiThrottler apiThrottler) : IArtistDiscoveryService
{
    private readonly ISpotifyService _spotifyService = spotifyService;
    private readonly ILastFmClient _lastFmClient = lastFmClient;
    private readonly IBandCachingService _cachingService = cachingService;
    private readonly IBandRepository _bandRepository = bandRepository;
    private readonly ILogger<ArtistDiscoveryService> _logger = logger;
    private readonly IApiThrottler _apiThrottler = apiThrottler;

    public async Task<List<SpotifyArtistDTO>?> GetArtistDetailsInBatches(string token, List<string> artistIds, int batchSize = 50)
    {
        var result = new List<SpotifyArtistDTO>();
        if (artistIds.Count == 0)
        {
            return result;
        }

        string cacheKey = $"artist-details:{string.Join(",", artistIds)}";
        if (_cachingService.TryGetCachedItem(cacheKey, out List<SpotifyArtistDTO>? cachedArtists))
        {
            return cachedArtists;
        }

        for (int i = 0; i < artistIds.Count; i += batchSize)
        {
            var batch = artistIds.Skip(i).Take(batchSize).ToList();
            if (batch.Count == 0)
            {
                continue;
            }

            try
            {
                var artistIdsParam = string.Join(",", batch);
                var artists = await _apiThrottler.ThrottledApiCall(() => _spotifyService.GetSeveralArtists(token, artistIdsParam));

                if (artists != null)
                {
                    result.AddRange(artists);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving artist details batch");
            }
        }

        _cachingService.CacheItem(cacheKey, result, TimeSpan.FromHours(6));
        return result;
    }

    async Task<List<BandModel>> IArtistDiscoveryService.FindArtistsByQuery(string token, List<string> genres, string queryTemplate, int limit, bool isNewRelease)
    {
        var result = new List<BandModel>();
        var processedArtistNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var genreBatches = BatchGenres(genres, 3);

        foreach (var genreBatch in genreBatches)
        {
            try
            {
                string cacheKey = $"artist-search:{queryTemplate}:{string.Join(",", genreBatch)}:{isNewRelease}";

                if (!_cachingService.TryGetCachedItem(cacheKey, out List<BandModel>? cachedResults))
                {
                    var batchQuery = BuildBatchQuery(queryTemplate, genreBatch);
                    var albumSearchResponse = await _apiThrottler.ThrottledApiCall(() => _spotifyService.SearchAsync(token, batchQuery, "album", limit: 50));

                    if (albumSearchResponse?.Albums?.Items == null)
                    {
                        continue;
                    }

                    var artistIds = albumSearchResponse.Albums.Items
                        .SelectMany(album => album.Artists)
                        .Select(artist => artist.Id)
                        .Distinct()
                        .ToList();

                    var artistAlbums = new Dictionary<string, SpotifyAlbumDTO>();
                    foreach (var album in albumSearchResponse.Albums.Items)
                    {
                        foreach (var artist in album.Artists)
                        {
                            if (!artistAlbums.ContainsKey(artist.Id))
                            {
                                artistAlbums[artist.Id] = album;
                            }
                        }
                    }

                    var artistDetails = await GetArtistDetailsInBatches(token, artistIds);

                    var undergroundArtists = artistDetails?
                        .Where(artist => artist.Popularity <= MusicDiscoveryConstants.MaxPopularityForUnderground)
                        .Take(limit / genreBatches.Count)
                        .ToList() ?? [];

                    var batchResults = new List<BandModel>();
                    foreach (var artist in undergroundArtists)
                    {
                        if (processedArtistNames.Contains(artist.Name))
                        {
                            continue;
                        }
                        processedArtistNames.Add(artist.Name);

                        var relatedAlbum = artistAlbums.TryGetValue(artist.Id, out var album) ? album : null;

                        var bandModel = new BandModel
                        {
                            Name = artist.Name,
                            Genres = artist.Genres?.ToList() ?? [],
                            ImageUrl = artist.Images?.OrderByDescending(img => img.Width * img.Height)
                                              .FirstOrDefault()?.Url ?? string.Empty,
                            IsFromSearch = true,
                            IsNewRelease = isNewRelease,
                            Popularity = artist.Popularity,
                            SearchTags = new List<string> { isNewRelease ? "new" : "hipster" },
                            LatestAlbum = relatedAlbum?.Name,
                            LatestAlbumReleaseDate = relatedAlbum?.ReleaseDate
                        };

                        batchResults.Add(bandModel);
                    }

                    _cachingService.CacheItem(cacheKey, batchResults, TimeSpan.FromHours(2));
                    result.AddRange(batchResults);
                }
                else if (cachedResults != null)
                {
                    foreach (var band in cachedResults)
                    {
                        if (!processedArtistNames.Contains(band.Name))
                        {
                            processedArtistNames.Add(band.Name);
                            result.Add(band);
                        }
                    }
                }

                if (result.Count >= limit)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for artists with query {QueryTemplate} for genres {GenreBatch}",
                    queryTemplate, string.Join(", ", genreBatch));
            }
        }

        if (isNewRelease && result.Count < limit / 2)
        {
            try
            {
                var currentYear = DateTime.Now.Year;
                var sixMonthsAgo = DateTime.Now.AddMonths(-6);
                var yearRange = sixMonthsAgo.Year == currentYear
                    ? currentYear.ToString()
                    : $"{sixMonthsAgo.Year}-{currentYear}";

                foreach (var genre in genres.Take(2))
                {
                    var yearQuery = $"genre:{genre} year:{yearRange}";
                    var recentAlbumSearchResponse = await _apiThrottler.ThrottledApiCall(() =>
                        _spotifyService.SearchAsync(token, yearQuery, "album", limit: 30));

                    if (recentAlbumSearchResponse?.Albums?.Items != null)
                    {
                        var yearArtistIds = recentAlbumSearchResponse.Albums.Items
                            .SelectMany(album => album.Artists)
                            .Select(artist => artist.Id)
                            .Distinct()
                            .ToList();

                        var yearArtistAlbums = new Dictionary<string, SpotifyAlbumDTO>();
                        foreach (var album in recentAlbumSearchResponse.Albums.Items)
                        {
                            foreach (var artist in album.Artists)
                            {
                                if (!yearArtistAlbums.ContainsKey(artist.Id))
                                {
                                    yearArtistAlbums[artist.Id] = album;
                                }
                            }
                        }

                        var yearArtistDetails = await GetArtistDetailsInBatches(token, yearArtistIds);

                        var recentUndergroundArtists = yearArtistDetails?
                            .Where(artist => artist.Popularity <= MusicDiscoveryConstants.MaxPopularityForUnderground)
                            .Take(limit / 4)
                            .ToList() ?? [];

                        foreach (var artist in recentUndergroundArtists)
                        {
                            if (processedArtistNames.Contains(artist.Name))
                            {
                                continue;
                            }
                            processedArtistNames.Add(artist.Name);

                            var relatedAlbum = yearArtistAlbums.TryGetValue(artist.Id, out var album) ? album : null;

                            var bandModel = new BandModel
                            {
                                Name = artist.Name,
                                Genres = artist.Genres?.ToList() ?? [],
                                ImageUrl = artist.Images?.OrderByDescending(img => img.Width * img.Height)
                                                  .FirstOrDefault()?.Url ?? string.Empty,
                                IsFromSearch = true,
                                IsNewRelease = true,
                                Popularity = artist.Popularity,
                                SearchTags = new List<string> { "new", "recent" },
                                LatestAlbum = relatedAlbum?.Name,
                                LatestAlbumReleaseDate = relatedAlbum?.ReleaseDate
                            };

                            result.Add(bandModel);
                        }

                        string recentCacheKey = $"recent-releases:{genre}:{yearRange}";
                        _cachingService.CacheItem(recentCacheKey, result.Where(b => b.SearchTags.Contains("recent")).ToList(), TimeSpan.FromHours(1));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for recent releases with year range");
            }
        }

        return result;
    }

    async Task<List<BandModel>> IArtistDiscoveryService.GetRegisteredBandsAsModels(List<string> genres, string location)
    {
        var bands = new List<BandModel>();
        var matchingBands = await GetMatchingRegisteredBands(genres, location);

        if (matchingBands == null || matchingBands.Count == 0)
        {
            return bands;
        }

        var bandModelTasks = matchingBands.Select(async band =>
        {
            var bandModel = new BandModel
            {
                Name = band.Name ?? "Unknown",
                Location = band.Country ?? band.City ?? location,
                Genres = !string.IsNullOrEmpty(band.Genre)
                    ? band.Genre.Split(',').Select(g => g.Trim()).ToList()
                    : new List<string>(),
                IsRegistered = true,
                ImageUrl = band.CoverImage != null ? $"/api/bands/{band.Id}/image" : string.Empty
            };

            try
            {
                string cacheKey = $"band:{bandModel.Name}";

                if (!_cachingService.TryGetCachedItem(cacheKey, out BandModel? cachedBandData))
                {
                    var lastFmData = await _apiThrottler.ThrottledApiCall(() => _lastFmClient.GetBandDataAsync(bandModel.Name));
                    if (lastFmData != null)
                    {
                        bandModel.Listeners = lastFmData.Listeners;
                        bandModel.PlayCount = lastFmData.PlayCount;

                        if (string.IsNullOrEmpty(bandModel.ImageUrl))
                        {
                            bandModel.ImageUrl = lastFmData.ImageUrl;
                        }

                        if (bandModel.Genres.Count == 0)
                        {
                            bandModel.Genres = lastFmData.Genres;
                        }
                        else if (lastFmData.Genres.Count > 0)
                        {
                            bandModel.Genres.AddRange(lastFmData.Genres.Except(bandModel.Genres, StringComparer.OrdinalIgnoreCase));
                        }

                        bandModel.SimilarArtists = lastFmData.SimilarArtists;
                        _cachingService.CacheItem(cacheKey, bandModel, TimeSpan.FromHours(24));
                    }
                }
                else if (cachedBandData != null)
                {
                    bandModel.Listeners = cachedBandData.Listeners;
                    bandModel.PlayCount = cachedBandData.PlayCount;

                    if (string.IsNullOrEmpty(bandModel.ImageUrl))
                    {
                        bandModel.ImageUrl = cachedBandData.ImageUrl;
                    }

                    if (bandModel.Genres.Count == 0)
                    {
                        bandModel.Genres = cachedBandData.Genres;
                    }
                    else if (cachedBandData.Genres.Count > 0)
                    {
                        bandModel.Genres.AddRange(cachedBandData.Genres.Except(bandModel.Genres, StringComparer.OrdinalIgnoreCase));
                    }

                    bandModel.SimilarArtists = cachedBandData.SimilarArtists;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enriching band data for {BandName}", band.Name);
            }

            return bandModel;
        }).ToArray();

        var bandModels = await Task.WhenAll(bandModelTasks);
        bands.AddRange(bandModels);

        return bands;
    }

    private async Task<List<Band>?> GetMatchingRegisteredBands(List<string> genres, string location)
    {
        var result = new List<Band>();

        try
        {
            string cacheKey = $"registered-bands:{string.Join(",", genres)}:{location}";

            if (_cachingService.TryGetCachedItem(cacheKey, out List<Band>? cachedBands))
            {
                return cachedBands;
            }

            if (genres.Count > 0 && !string.IsNullOrEmpty(location))
            {
                foreach (var genre in genres)
                {
                    var bands = await _bandRepository.GetBandsByGenreAndLocation(genre, location);
                    foreach (var band in bands)
                    {
                        if (!result.Any(b => b.Id == band.Id))
                        {
                            result.Add(band);
                        }
                    }
                }

                if (result.Count == 0)
                {
                    result = await _bandRepository.GetBandsByLocation(location);

                    if (result.Count == 0)
                    {
                        foreach (var genre in genres)
                        {
                            var bands = await _bandRepository.GetBandsByGenre(genre);
                            foreach (var band in bands)
                            {
                                if (!result.Any(b => b.Id == band.Id))
                                {
                                    result.Add(band);
                                }
                            }
                        }
                    }
                }
            }
            else if (genres.Count > 0)
            {
                foreach (var genre in genres)
                {
                    var bands = await _bandRepository.GetBandsByGenre(genre);
                    foreach (var band in bands)
                    {
                        if (!result.Any(b => b.Id == band.Id))
                        {
                            result.Add(band);
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(location))
            {
                result = await _bandRepository.GetBandsByLocation(location);
            }
            else
            {
                result = await _bandRepository.GetAllBands();
            }

            if (result.Count == 0)
            {
                result = await _bandRepository.GetAllBands();
            }

            _cachingService.CacheItem(cacheKey, result, TimeSpan.FromMinutes(30));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting registered bands");
            return [];
        }

        return result;
    }

    private List<List<string>> BatchGenres(List<string> genres, int batchSize)
    {
        var result = new List<List<string>>();
        for (int i = 0; i < genres.Count; i += batchSize)
        {
            result.Add(genres.Skip(i).Take(batchSize).ToList());
        }
        return result.Count > 0 ? result : new List<List<string>> { new List<string>() };
    }

    private string BuildBatchQuery(string queryTemplate, List<string> genres)
    {
        if (queryTemplate.Contains("{genre}"))
        {
            var genreQuery = string.Join(" OR ", genres.Select(g => $"genre:\"{g}\""));
            return queryTemplate.Replace("{genre}", genreQuery);
        }
        return queryTemplate;
    }
}
