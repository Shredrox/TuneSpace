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

    public async Task<List<SpotifyArtistDTO>?> GetArtistDetailsInBatchesAsync(string token, List<string> artistIds, int batchSize = 50)
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
                var artists = await _apiThrottler.ThrottledApiCall(() => _spotifyService.GetSeveralArtistsAsync(token, artistIdsParam));

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

    async Task<List<BandModel>> IArtistDiscoveryService.FindArtistsByQueryAsync(string token, List<string> genres, string queryTemplate, int limit, bool isNewRelease)
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
                    var artistSearchResponse = await _apiThrottler.ThrottledApiCall(() => _spotifyService.SearchAsync(token, batchQuery, "artist", limit: 50));

                    if (artistSearchResponse?.Artists?.Items == null)
                    {
                        continue;
                    }

                    var artistDetails = artistSearchResponse.Artists.Items;

                    var artistAlbums = new Dictionary<string, SpotifyAlbumDTO>();
                    if (isNewRelease)
                    {
                        var currentYear = DateTime.Now.Year;
                        var lastYear = currentYear - 1;
                        var yearRange = $"{lastYear}-{currentYear}";

                        foreach (var artist in artistDetails.Take(10))
                        {
                            try
                            {
                                var albumQuery = $"artist:\"{artist.Name}\" year:{yearRange}";
                                var albumSearchResponse = await _apiThrottler.ThrottledApiCall(() =>
                                    _spotifyService.SearchAsync(token, albumQuery, "album", limit: 5));

                                if (albumSearchResponse?.Albums?.Items != null && albumSearchResponse.Albums.Items.Any())
                                {
                                    var latestAlbum = albumSearchResponse.Albums.Items
                                        .OrderByDescending(a => a.ReleaseDate)
                                        .FirstOrDefault();

                                    if (latestAlbum != null)
                                    {
                                        artistAlbums[artist.Id] = latestAlbum;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Error fetching albums for artist {ArtistName}", artist.Name);
                            }
                        }
                    }

                    var undergroundArtists = artistDetails?
                        .Where(artist => artist.Popularity <= MusicDiscoveryConstants.MaxPopularityForUnderground)
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
                            SearchTags = [isNewRelease ? "new" : "hipster"],
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
                foreach (var genre in genres.Take(2))
                {
                    var newAlbumQuery = $"genre:\"{genre}\" tag:new";
                    var newAlbumSearchResponse = await _apiThrottler.ThrottledApiCall(() =>
                        _spotifyService.SearchAsync(token, newAlbumQuery, "album", limit: 20));

                    if (newAlbumSearchResponse?.Albums?.Items != null)
                    {
                        var newArtistIds = newAlbumSearchResponse.Albums.Items
                            .SelectMany(album => album.Artists)
                            .Select(artist => artist.Id)
                            .Distinct()
                            .ToList();

                        var newArtistAlbums = new Dictionary<string, SpotifyAlbumDTO>();
                        foreach (var album in newAlbumSearchResponse.Albums.Items)
                        {
                            foreach (var artist in album.Artists)
                            {
                                if (!newArtistAlbums.ContainsKey(artist.Id))
                                {
                                    newArtistAlbums[artist.Id] = album;
                                }
                            }
                        }

                        var newArtistDetails = await GetArtistDetailsInBatchesAsync(token, newArtistIds);

                        var newUndergroundArtists = newArtistDetails?
                            .Where(artist => artist.Popularity <= MusicDiscoveryConstants.MaxPopularityForUnderground)
                            .Take(limit / 4)
                            .ToList() ?? [];

                        foreach (var artist in newUndergroundArtists)
                        {
                            if (processedArtistNames.Contains(artist.Name))
                            {
                                continue;
                            }
                            processedArtistNames.Add(artist.Name);

                            var relatedAlbum = newArtistAlbums.TryGetValue(artist.Id, out var album) ? album : null;

                            var bandModel = new BandModel
                            {
                                Name = artist.Name,
                                Genres = artist.Genres?.ToList() ?? [],
                                ImageUrl = artist.Images?.OrderByDescending(img => img.Width * img.Height)
                                                  .FirstOrDefault()?.Url ?? string.Empty,
                                IsFromSearch = true,
                                IsNewRelease = true,
                                Popularity = artist.Popularity,
                                SearchTags = ["new", "recent"],
                                LatestAlbum = relatedAlbum?.Name,
                                LatestAlbumReleaseDate = relatedAlbum?.ReleaseDate
                            };

                            result.Add(bandModel);
                        }

                        string recentCacheKey = $"new-releases:{genre}";
                        _cachingService.CacheItem(recentCacheKey, result.Where(b => b.SearchTags.Contains("recent")).ToList(), TimeSpan.FromHours(1));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for new releases with tag:new filter");
            }
        }

        return result;
    }

    async Task<List<BandModel>> IArtistDiscoveryService.FindHipsterAndNewArtistsAsync(string token, string queryTemplate, int limit)
    {
        var result = new List<BandModel>();
        var processedArtistNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            var hipsterCacheKey = "hipster-artists";
            if (!_cachingService.TryGetCachedItem(hipsterCacheKey, out List<BandModel>? cachedHipsterArtists))
            {
                var hipsterQuery = "tag:hipster";
                var hipsterSearchResponse = await _apiThrottler.ThrottledApiCall(() =>
                    _spotifyService.SearchAsync(token, hipsterQuery, "album", limit / 2));

                if (hipsterSearchResponse?.Albums?.Items != null)
                {
                    foreach (var album in hipsterSearchResponse.Albums.Items)
                    {
                        foreach (var artist in album.Artists)
                        {
                            if (!string.IsNullOrEmpty(artist.Name) &&
                                !processedArtistNames.Contains(artist.Name) &&
                                processedArtistNames.Count < limit)
                            {
                                processedArtistNames.Add(artist.Name);
                                var bandModel = new BandModel
                                {
                                    Id = artist.Id,
                                    Name = artist.Name,
                                    ImageUrl = artist.Images?.FirstOrDefault()?.Url ?? album.Images?.FirstOrDefault()?.Url ?? string.Empty,
                                    SearchTags = new List<string> { "hipster" },
                                    LatestAlbum = album.Name,
                                    LatestAlbumReleaseDate = album.ReleaseDate
                                };
                                result.Add(bandModel);
                            }
                        }
                    }
                }

                _cachingService.CacheItem(hipsterCacheKey, result.Where(b => b.SearchTags.Contains("hipster")).ToList(), TimeSpan.FromHours(2));
            }
            else if (cachedHipsterArtists != null)
            {
                result.AddRange(cachedHipsterArtists);
                foreach (var artist in cachedHipsterArtists)
                {
                    processedArtistNames.Add(artist.Name);
                }
            }

            var newCacheKey = "new-artists";
            if (!_cachingService.TryGetCachedItem(newCacheKey, out List<BandModel>? cachedNewArtists))
            {
                var newQuery = "tag:new";
                var newSearchResponse = await _apiThrottler.ThrottledApiCall(() =>
                    _spotifyService.SearchAsync(token, newQuery, "album", limit / 2));

                if (newSearchResponse?.Albums?.Items != null)
                {
                    var newArtists = new List<BandModel>();
                    foreach (var album in newSearchResponse.Albums.Items)
                    {
                        foreach (var artist in album.Artists)
                        {
                            if (!string.IsNullOrEmpty(artist.Name) &&
                                !processedArtistNames.Contains(artist.Name) &&
                                result.Count < limit)
                            {
                                processedArtistNames.Add(artist.Name);
                                var bandModel = new BandModel
                                {
                                    Id = artist.Id,
                                    Name = artist.Name,
                                    ImageUrl = artist.Images?.FirstOrDefault()?.Url ?? album.Images?.FirstOrDefault()?.Url ?? string.Empty,
                                    SearchTags = new List<string> { "new" },
                                    LatestAlbum = album.Name,
                                    LatestAlbumReleaseDate = album.ReleaseDate
                                };
                                newArtists.Add(bandModel);
                            }
                        }
                    }
                    result.AddRange(newArtists);
                    _cachingService.CacheItem(newCacheKey, newArtists, TimeSpan.FromHours(1));
                }
            }
            else if (cachedNewArtists != null)
            {
                result.AddRange(cachedNewArtists.Where(a => !processedArtistNames.Contains(a.Name)));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for hipster and new artists");
        }

        return [.. result.Take(limit)];
    }

    async Task<List<BandModel>> IArtistDiscoveryService.GetRegisteredBandsAsModelsAsync(List<string> genres, string location)
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
                Id = band.Id.ToString(),
                Name = band.Name ?? "Unknown",
                Location = band.Country ?? band.City ?? location,
                Genres = !string.IsNullOrEmpty(band.Genre)
                    ? [.. band.Genre.Split(',').Select(g => g.Trim())]
                    : [],
                IsRegistered = true,
                CoverImage = band.CoverImage ?? [],
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
                    var bands = await _bandRepository.GetBandsByGenreAndLocationAsync(genre, location);
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
                    result = await _bandRepository.GetBandsByLocationAsync(location);

                    if (result.Count == 0)
                    {
                        foreach (var genre in genres)
                        {
                            var bands = await _bandRepository.GetBandsByGenreAsync(genre);
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
                    var bands = await _bandRepository.GetBandsByGenreAsync(genre);
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
                result = await _bandRepository.GetBandsByLocationAsync(location);
            }
            else
            {
                result = await _bandRepository.GetAllBandsAsync();
            }

            if (result.Count == 0)
            {
                result = await _bandRepository.GetAllBandsAsync();
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

    private static List<List<string>> BatchGenres(List<string> genres, int batchSize)
    {
        var result = new List<List<string>>();
        for (int i = 0; i < genres.Count; i += batchSize)
        {
            result.Add(genres.Skip(i).Take(batchSize).ToList());
        }
        return result.Count > 0 ? result : [[]];
    }

    private static string BuildBatchQuery(string queryTemplate, List<string> genres)
    {
        if (queryTemplate.Contains("{genre}"))
        {
            // Use OR operator to search across multiple genres more effectively
            var genreQuery = string.Join(" OR ", genres.Select(g => $"genre:\"{g}\""));
            return queryTemplate.Replace("{genre}", genreQuery);
        }
        return queryTemplate;
    }
}
