using TuneSpace.Core.DTOs.Responses.Spotify;
using TuneSpace.Core.Interfaces.IClients;
using TuneSpace.Core.Interfaces.IServices;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Models;
using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Services;

internal class MusicDiscoveryService(
    IMusicBrainzClient musicBrainzClient,
    ILastFmClient lastFmClient,
    ISpotifyService spotifyService,
    IBandRepository bandRepository) : IMusicDiscoveryService
{
    private static readonly Dictionary<string, DateTime> PreviouslyRecommendedBands = new();
    private static readonly Dictionary<string, DateTime> RegisteredBandDates = new();

    async Task<List<BandModel>> IMusicDiscoveryService.GetBandRecommendationsAsync(string spotifyAccessToken, List<string> genres, string location)
    {
        var recentlyPlayedTask = spotifyService.GetUserRecentlyPlayedTracks(spotifyAccessToken);
        var followedArtistsTask = spotifyService.GetUserFollowedArtists(spotifyAccessToken);
        var topArtistsTask = spotifyService.GetUserTopArtists(spotifyAccessToken);

        await Task.WhenAll(recentlyPlayedTask, followedArtistsTask, topArtistsTask);

        var recentlyPlayed = await recentlyPlayedTask;
        var followedArtists = await followedArtistsTask;
        var topArtists = await topArtistsTask;

        var knownArtistNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var artist in followedArtists)
        {
            knownArtistNames.Add(artist.Name);
        }

        foreach (var artist in topArtists)
        {
            knownArtistNames.Add(artist.Name);
        }

        var extractedGenres = followedArtists
            .SelectMany(artist => artist.Genres ?? new List<string>())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var recentlyPlayedArtistIds = recentlyPlayed
            .Select(track => track.ArtistId)
            .Where(id => !string.IsNullOrEmpty(id))
            .Distinct()
            .ToList();

        var recentArtistsWithGenres = new List<SpotifyArtistDTO>();
        if (recentlyPlayedArtistIds.Count != 0)
        {
            string artistIdsParam = string.Join(",", recentlyPlayedArtistIds);
            var artistDetails = await spotifyService.GetSeveralArtists(spotifyAccessToken, artistIdsParam);
            if (artistDetails != null)
            {
                recentArtistsWithGenres.AddRange(artistDetails);
            }
        }

        var recentlyPlayedGenres = recentArtistsWithGenres
            .SelectMany(artist => artist.Genres ?? new List<string>())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var combinedGenres = new List<string>();
        combinedGenres.AddRange(recentlyPlayedGenres);
        combinedGenres.AddRange(extractedGenres.Where(g => !combinedGenres.Contains(g, StringComparer.OrdinalIgnoreCase)));

        genres = combinedGenres.Count > 0 ? combinedGenres : genres;

        var undergroundArtistsTask = FindUndergroundArtistsByGenres(spotifyAccessToken, genres);
        var newReleasesArtistsTask = FindNewUndergroundReleases(spotifyAccessToken, genres);
        var localBandsTask = musicBrainzClient.GetBandsByLocationAsync(location, 20, genres);
        var registeredBandsTask = GetRegisteredBandsAsModels(genres, location);

        await Task.WhenAll(localBandsTask, registeredBandsTask, undergroundArtistsTask, newReleasesArtistsTask);

        var localBands = await localBandsTask;
        var registeredBands = await registeredBandsTask;
        var undergroundArtists = await undergroundArtistsTask;
        var newReleaseArtists = await newReleasesArtistsTask;

        undergroundArtists.AddRange(newReleaseArtists);

        localBands = await lastFmClient.EnrichBandData(localBands);
        undergroundArtists = await lastFmClient.EnrichBandData(undergroundArtists);

        var similarBands = new List<BandModel>();
        var processedBandNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        localBands.ForEach(b => processedBandNames.Add(b.Name));
        registeredBands.ForEach(b => processedBandNames.Add(b.Name));
        undergroundArtists.ForEach(b => processedBandNames.Add(b.Name));

        var similarBandTasks = new List<Task<List<BandModel>>>();
        foreach (var artist in topArtists.Take(5))
        {
            similarBandTasks.Add(GetSimilarBandsForArtist(artist.Name, processedBandNames));
        }

        var similarBandResults = await Task.WhenAll(similarBandTasks);
        foreach (var bandList in similarBandResults)
        {
            similarBands.AddRange(bandList);
            foreach (var band in bandList)
            {
                processedBandNames.Add(band.Name);
            }
        }

        var similarToRegisteredBandTasks = new List<Task<List<BandModel>>>();
        foreach (var band in registeredBands.Take(5))
        {
            similarToRegisteredBandTasks.Add(GetSimilarBandsForRegisteredBand(band.Name, processedBandNames));
        }

        var similarToRegisteredResults = await Task.WhenAll(similarToRegisteredBandTasks);
        foreach (var bandList in similarToRegisteredResults)
        {
            similarBands.AddRange(bandList);
        }

        var recommendedBands = new List<BandModel>();
        recommendedBands.AddRange(ScoreBands(localBands, genres, location, topArtists, isRegistered: false));
        recommendedBands.AddRange(ScoreBands(similarBands, genres, location, topArtists, isRegistered: false));
        recommendedBands.AddRange(ScoreBands(undergroundArtists, genres, location, topArtists, isRegistered: false, isFromSearch: true));
        recommendedBands.AddRange(ScoreBands(registeredBands, genres, location, topArtists, isRegistered: true));

        var now = DateTime.UtcNow;
        var keysToRemove = PreviouslyRecommendedBands
            .Where(kvp => (now - kvp.Value).TotalDays > MusicDiscoveryConstants.RecommendationCooldownDays)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            PreviouslyRecommendedBands.Remove(key);
        }

        var finalRecommendations = ApplyDiversityAndExploration(recommendedBands);

        finalRecommendations = finalRecommendations
            .Where(band => !knownArtistNames.Contains(band.Name))
            .ToList();

        foreach (var band in finalRecommendations)
        {
            PreviouslyRecommendedBands[band.Name] = now;
        }

        return finalRecommendations;
    }

    private async Task<List<BandModel>> FindUndergroundArtistsByGenres(string token, List<string> genres)
    {
        var result = new List<BandModel>();
        var processedArtistNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        //var selectedGenres = genres.Take(Math.Min(5, genres.Count)).ToList();

        foreach (var genre in genres)
        {
            try
            {
                var albumSearchResponse = await spotifyService.SearchAsync(
                    token,
                    $"genre:{genre} tag:hipster",
                    "album",
                    limit: 50);

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

                const int chunkSize = 50;
                for (int i = 0; i < artistIds.Count; i += chunkSize)
                {
                    var idsChunk = artistIds.Skip(i).Take(chunkSize).ToList();
                    if (idsChunk.Count == 0) continue;

                    var artistIdsParam = string.Join(",", idsChunk);
                    var artistDetails = await spotifyService.GetSeveralArtists(token, artistIdsParam);

                    if (artistDetails == null) continue;

                    var undergroundArtists = artistDetails
                        .Where(artist => artist.Popularity <= MusicDiscoveryConstants.MaxPopularityForUnderground)
                        .Take(10)
                        .ToList();

                    foreach (var artist in undergroundArtists)
                    {
                        if (processedArtistNames.Contains(artist.Name)) continue;
                        processedArtistNames.Add(artist.Name);

                        var relatedAlbum = artistAlbums.TryGetValue(artist.Id, out var album) ? album : null;

                        var bandModel = new BandModel
                        {
                            Name = artist.Name,
                            Genres = artist.Genres?.ToList() ?? new List<string>(),
                            ImageUrl = artist.Images?.OrderByDescending(img => img.Width * img.Height)
                                              .FirstOrDefault()?.Url ?? string.Empty,
                            IsFromSearch = true,
                            Popularity = artist.Popularity,
                            SearchTags = new List<string> { "hipster" },
                            LatestAlbum = relatedAlbum?.Name,
                            LatestAlbumReleaseDate = relatedAlbum?.ReleaseDate
                        };

                        result.Add(bandModel);

                        if (result.Count >= MusicDiscoveryConstants.UndergroundArtistsToFetch)
                            break;
                    }

                    if (result.Count >= MusicDiscoveryConstants.UndergroundArtistsToFetch)
                        break;
                }

                if (result.Count >= MusicDiscoveryConstants.UndergroundArtistsToFetch)
                    break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching for underground artists in genre {genre}: {ex.Message}");
                continue;
            }
        }

        return result;
    }

    private async Task<List<BandModel>> FindNewUndergroundReleases(string token, List<string> genres)
    {
        var result = new List<BandModel>();
        var processedArtistNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        //var selectedGenres = genres.Take(Math.Min(3, genres.Count)).ToList();
        var selectedGenres = genres;

        foreach (var genre in selectedGenres)
        {
            try
            {
                var albumSearchResponse = await spotifyService.SearchAsync(
                    token,
                    $"genre:{genre} tag:new",
                    "album",
                    limit: 50);

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

                const int chunkSize = 50;
                for (int i = 0; i < artistIds.Count; i += chunkSize)
                {
                    var idsChunk = artistIds.Skip(i).Take(chunkSize).ToList();
                    if (idsChunk.Count == 0) continue;

                    var artistIdsParam = string.Join(",", idsChunk);
                    var artistDetails = await spotifyService.GetSeveralArtists(token, artistIdsParam);

                    if (artistDetails == null) continue;

                    var undergroundArtists = artistDetails
                        .Where(artist => artist.Popularity <= MusicDiscoveryConstants.MaxPopularityForUnderground)
                        .Take(10)
                        .ToList();

                    foreach (var artist in undergroundArtists)
                    {
                        if (processedArtistNames.Contains(artist.Name)) continue;
                        processedArtistNames.Add(artist.Name);

                        var relatedAlbum = artistAlbums.TryGetValue(artist.Id, out var album) ? album : null;

                        var bandModel = new BandModel
                        {
                            Name = artist.Name,
                            Genres = artist.Genres?.ToList() ?? new List<string>(),
                            ImageUrl = artist.Images?.OrderByDescending(img => img.Width * img.Height)
                                              .FirstOrDefault()?.Url ?? string.Empty,
                            IsFromSearch = true,
                            IsNewRelease = true,
                            Popularity = artist.Popularity,
                            SearchTags = new List<string> { "new" },
                            LatestAlbum = relatedAlbum?.Name,
                            LatestAlbumReleaseDate = relatedAlbum?.ReleaseDate
                        };

                        result.Add(bandModel);

                        if (result.Count >= MusicDiscoveryConstants.UndergroundArtistsToFetch / 2)
                            break;
                    }

                    if (result.Count >= MusicDiscoveryConstants.UndergroundArtistsToFetch / 2)
                        break;
                }

                if (result.Count >= MusicDiscoveryConstants.UndergroundArtistsToFetch / 2)
                    break;

                if (result.Count < MusicDiscoveryConstants.UndergroundArtistsToFetch / 2)
                {
                    var currentYear = DateTime.Now.Year;
                    var sixMonthsAgo = DateTime.Now.AddMonths(-6);
                    var yearRange = sixMonthsAgo.Year == currentYear
                        ? currentYear.ToString()
                        : $"{sixMonthsAgo.Year}-{currentYear}";

                    var recentAlbumSearchResponse = await spotifyService.SearchAsync(
                        token,
                        $"genre:{genre} year:{yearRange}",
                        "album",
                        limit: 50);

                    if (recentAlbumSearchResponse?.Albums?.Items != null)
                    {
                        var recentArtistIds = recentAlbumSearchResponse.Albums.Items
                            .SelectMany(album => album.Artists)
                            .Select(artist => artist.Id)
                            .Where(id => !artistIds.Contains(id))
                            .Distinct()
                            .ToList();

                        artistIds.AddRange(recentArtistIds);

                        foreach (var album in recentAlbumSearchResponse.Albums.Items)
                        {
                            foreach (var artist in album.Artists)
                            {
                                if (!artistAlbums.ContainsKey(artist.Id))
                                {
                                    artistAlbums[artist.Id] = album;
                                }
                            }
                        }

                        // Continue with the same processing for these additional artists
                        // (This would need to be implemented similar to the previous chunk processing)
                        // Omitted for brevity but would follow same pattern as above
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching for new releases in genre {genre}: {ex.Message}");
                continue;
            }
        }

        return result;
    }

    private List<BandModel> ScoreBands(List<BandModel> bands, List<string> userGenres, string location,
                                     List<TopArtistDTO> topArtists, bool isRegistered, bool isFromSearch = false)
    {
        foreach (var band in bands)
        {
            double score = 0;

            if (userGenres.Count > 0 && band.Genres.Count > 0)
            {
                var genreMatchCount = band.Genres
                    .Count(g => userGenres.Any(ug =>
                        ug.Equals(g, StringComparison.OrdinalIgnoreCase) ||
                        g.Contains(ug, StringComparison.OrdinalIgnoreCase) ||
                        ug.Contains(g, StringComparison.OrdinalIgnoreCase)));

                score += genreMatchCount * 0.3;
            }

            if (!string.IsNullOrEmpty(band.Location) && band.Location.Equals(location, StringComparison.OrdinalIgnoreCase))
            {
                score += 0.3;
            }

            score += (1.0 - (band.Listeners / 1000000.0)) * 0.2;

            foreach (var artist in topArtists)
            {
                if (band.SimilarArtists.Any(sa => sa.Equals(artist.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    score += 0.2;
                    break;
                }
            }

            if (band.IsLesserKnown)
            {
                score += MusicDiscoveryConstants.UndergroundBandBonus;
            }

            if (isRegistered)
            {
                score += MusicDiscoveryConstants.RegisteredBandBonus;
                band.IsRegistered = true;

                var random = new Random();
                if (random.NextDouble() < 0.3 && !RegisteredBandDates.ContainsKey(band.Name))
                {
                    RegisteredBandDates[band.Name] = DateTime.UtcNow.AddDays(-random.Next(1, 29));
                }

                if (RegisteredBandDates.TryGetValue(band.Name, out DateTime registrationDate))
                {
                    double daysSinceRegistration = (DateTime.UtcNow - registrationDate).TotalDays;
                    if (daysSinceRegistration <= 30)
                    {
                        score += MusicDiscoveryConstants.NewRegistrationBonus;
                    }
                }
            }

            if (isFromSearch && band.IsLesserKnown)
            {
                if (band.Popularity <= 20)
                {
                    score += MusicDiscoveryConstants.UndergroundBandBonus * 1.5;
                }
                else
                {
                    score += MusicDiscoveryConstants.UndergroundBandBonus;
                }

                if (userGenres.Count > 0 && band.Genres.Count > 0)
                {
                    var genreMatchCount = band.Genres
                        .Count(g => userGenres.Any(ug =>
                            ug.Equals(g, StringComparison.OrdinalIgnoreCase) ||
                            g.Contains(ug, StringComparison.OrdinalIgnoreCase) ||
                            ug.Contains(g, StringComparison.OrdinalIgnoreCase)));

                    score += genreMatchCount * 0.15;
                }

                if (band.IsNewRelease)
                {
                    score += 0.2;
                }

                if (band.SearchTags?.Contains("hipster") == true)
                {
                    score += 0.1;
                }
            }

            band.RelevanceScore = Math.Min(score, 1.0);
        }

        return bands;
    }

    private async Task<List<BandModel>> GetRegisteredBandsAsModels(List<string> genres, string location)
    {
        var bands = new List<BandModel>();
        var matchingBands = await GetMatchingRegisteredBands(genres, location);

        foreach (var band in matchingBands)
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
                var lastFmData = await lastFmClient.GetBandDataAsync(bandModel.Name);
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
                    bandModel.Genres.AddRange(lastFmData.Genres.Except(bandModel.Genres));
                }

                bandModel.SimilarArtists = await lastFmClient.GetSimilarBandsAsync(bandModel.Name, 5);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enriching band data for {band.Name}: {ex.Message}");
            }

            bands.Add(bandModel);
        }

        return bands;
    }

    private async Task<List<Band>> GetMatchingRegisteredBands(List<string> genres, string location)
    {
        var result = new List<Band>();

        try
        {
            if (genres.Count > 0 && !string.IsNullOrEmpty(location))
            {
                foreach (var genre in genres)
                {
                    var bands = await bandRepository.GetBandsByGenreAndLocation(genre, location);
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
                    result = await bandRepository.GetBandsByLocation(location);

                    if (result.Count == 0)
                    {
                        foreach (var genre in genres)
                        {
                            var bands = await bandRepository.GetBandsByGenre(genre);
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
                    var bands = await bandRepository.GetBandsByGenre(genre);
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
                result = await bandRepository.GetBandsByLocation(location);
            }
            else
            {
                result = await bandRepository.GetAllBands();
            }

            if (result.Count == 0)
            {
                result = await bandRepository.GetAllBands();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting registered bands: {ex.Message}");
            return new List<Band>();
        }

        return result;
    }

    private List<BandModel> ApplyDiversityAndExploration(List<BandModel> recommendedBands)
    {
        var now = DateTime.UtcNow;
        var result = new List<BandModel>();
        var genreCounters = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        var registeredBands = recommendedBands.Where(b => b.IsRegistered).ToList();
        var externalBands = recommendedBands.Where(b => !b.IsRegistered).ToList();

        // First pass - exclude recently recommended bands unless they're highly relevant
        var candidateRegisteredBands = registeredBands
            .Where(b => !PreviouslyRecommendedBands.ContainsKey(b.Name) ||
                       b.RelevanceScore > 0.7)
            .OrderByDescending(b => b.RelevanceScore)
            .ToList();

        var candidateExternalBands = externalBands
            .Where(b => !PreviouslyRecommendedBands.ContainsKey(b.Name) ||
                       b.RelevanceScore > 0.8)
            .OrderByDescending(b => b.RelevanceScore)
            .ToList();

        // Second pass - calculate diversity scores
        CalculateDiversityScores(candidateRegisteredBands, genreCounters, now);
        CalculateDiversityScores(candidateExternalBands, genreCounters, now);

        int minRegisteredBandsCount = (int)Math.Ceiling(MusicDiscoveryConstants.MaxRecommendations * MusicDiscoveryConstants.MinRegisteredBandPercentage);
        int registeredBandsToInclude = Math.Min(minRegisteredBandsCount, candidateRegisteredBands.Count);
        int externalBandsToInclude = MusicDiscoveryConstants.MaxRecommendations - registeredBandsToInclude;

        var selectedRegisteredBands = candidateRegisteredBands
            .OrderByDescending(b => b.DiversityScore)
            .Take(registeredBandsToInclude)
            .ToList();

        foreach (var band in selectedRegisteredBands)
        {
            result.Add(band);

            foreach (var genre in band.Genres)
            {
                if (!genreCounters.ContainsKey(genre))
                {
                    genreCounters[genre] = 0;
                }
                genreCounters[genre]++;
            }
        }

        CalculateDiversityScores(candidateExternalBands, genreCounters, now);

        var selectedExternalBands = candidateExternalBands
            .OrderByDescending(b => b.DiversityScore)
            .Take(externalBandsToInclude)
            .ToList();

        foreach (var band in selectedExternalBands)
        {
            result.Add(band);

            foreach (var genre in band.Genres)
            {
                if (!genreCounters.ContainsKey(genre))
                {
                    genreCounters[genre] = 0;
                }
                genreCounters[genre]++;
            }
        }

        return result;
    }

    private void CalculateDiversityScores(List<BandModel> bands, Dictionary<string, int> genreCounters, DateTime now)
    {
        foreach (var band in bands)
        {
            double diversityScore = band.RelevanceScore;

            foreach (var genre in band.Genres)
            {
                if (genreCounters.TryGetValue(genre, out int count) && count > 0)
                {
                    diversityScore -= count * MusicDiscoveryConstants.DiversityFactor;
                }
            }

            if (PreviouslyRecommendedBands.TryGetValue(band.Name, out DateTime lastRecommended))
            {
                double daysSinceLastRecommended = Math.Max(1, (now - lastRecommended).TotalDays);

                double timeDecayFactor = Math.Min(1.0, daysSinceLastRecommended / MusicDiscoveryConstants.RecommendationCooldownDays);
                diversityScore *= timeDecayFactor;
            }

            if (band.RelevanceScore < 0.7)
            {
                var random = new Random();
                diversityScore += random.NextDouble() * MusicDiscoveryConstants.ExplorationFactor;
            }

            band.DiversityScore = Math.Max(0, Math.Min(1.0, diversityScore));
        }
    }

    private async Task<List<BandModel>> GetSimilarBandsForArtist(string artistName, HashSet<string> processedBandNames)
    {
        var result = new List<BandModel>();
        try
        {
            var similarArtistNames = await lastFmClient.GetSimilarBandsAsync(artistName, 7);
            foreach (var similarArtistName in similarArtistNames)
            {
                if (processedBandNames.Contains(similarArtistName))
                {
                    continue;
                }

                try
                {
                    var band = await musicBrainzClient.GetBandDataAsync(similarArtistName);
                    band = await lastFmClient.GetBandDataAsync(similarArtistName);

                    band.SimilarToArtistName = artistName;

                    result.Add(band);
                }
                catch
                {
                    continue;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting similar bands for {artistName}: {ex.Message}");
        }

        return result;
    }

    private async Task<List<BandModel>> GetSimilarBandsForRegisteredBand(string bandName, HashSet<string> processedBandNames)
    {
        var result = new List<BandModel>();
        try
        {
            var similarArtistNames = await lastFmClient.GetSimilarBandsAsync(bandName, 3);
            foreach (var similarArtistName in similarArtistNames)
            {
                if (processedBandNames.Contains(similarArtistName))
                {
                    continue;
                }

                try
                {
                    var band = await musicBrainzClient.GetBandDataAsync(similarArtistName);
                    band = await lastFmClient.GetBandDataAsync(similarArtistName);

                    band.SimilarToRegisteredBand = bandName;

                    result.Add(band);
                    processedBandNames.Add(similarArtistName);
                }
                catch
                {
                    continue;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting similar bands for registered band {bandName}: {ex.Message}");
        }

        return result;
    }
}
