using TuneSpace.Core.Interfaces.IClients;
using TuneSpace.Core.Interfaces.IServices;
using TuneSpace.Core.Models;
using TuneSpace.Core.Common;
using System.Collections.Concurrent;

namespace TuneSpace.Application.Services;

internal class MusicDiscoveryService(
    IMusicBrainzClient musicBrainzClient,
    ISpotifyService spotifyService,
    IArtistDiscoveryService artistDiscoveryService,
    IDataEnrichmentService dataEnrichmentService,
    IRecommendationScoringService scoringService) : IMusicDiscoveryService
{
    private static readonly ConcurrentDictionary<string, DateTime> PreviouslyRecommendedBands = new();
    private readonly IMusicBrainzClient _musicBrainzClient = musicBrainzClient;
    private readonly ISpotifyService _spotifyService = spotifyService;
    private readonly IArtistDiscoveryService _artistDiscoveryService = artistDiscoveryService;
    private readonly IDataEnrichmentService _dataEnrichmentService = dataEnrichmentService;
    private readonly IRecommendationScoringService _scoringService = scoringService;

    async Task<List<BandModel>> IMusicDiscoveryService.GetBandRecommendationsAsync(string spotifyAccessToken, List<string> genres, string location)
    {
        var recentlyPlayedTask = _spotifyService.GetUserRecentlyPlayedTracks(spotifyAccessToken);
        var followedArtistsTask = _spotifyService.GetUserFollowedArtists(spotifyAccessToken);
        var topArtistsTask = _spotifyService.GetUserTopArtists(spotifyAccessToken);

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

        var recentArtistsWithGenres = await _artistDiscoveryService.GetArtistDetailsInBatches(spotifyAccessToken, recentlyPlayedArtistIds);

        var recentlyPlayedGenres = recentArtistsWithGenres?
            .SelectMany(artist => artist.Genres ?? [])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var combinedGenres = new List<string>();
        if (recentlyPlayedGenres != null)
        {
            combinedGenres.AddRange(recentlyPlayedGenres);
        }
        combinedGenres.AddRange(extractedGenres.Where(g => !combinedGenres.Contains(g, StringComparer.OrdinalIgnoreCase)));

        genres = combinedGenres.Count > 0 ? combinedGenres : genres;

        var undergroundArtistsTask = _artistDiscoveryService.FindArtistsByQuery(
            spotifyAccessToken, genres, "genre:{genre} tag:hipster", MusicDiscoveryConstants.UndergroundArtistsToFetch);

        var newReleasesArtistsTask = _artistDiscoveryService.FindArtistsByQuery(
            spotifyAccessToken, genres, "genre:{genre} tag:new", MusicDiscoveryConstants.UndergroundArtistsToFetch / 2, true);

        var localBandsTask = _musicBrainzClient.GetBandsByLocationAsync(location, 20, genres);
        var registeredBandsTask = _artistDiscoveryService.GetRegisteredBandsAsModels(genres, location);

        await Task.WhenAll(localBandsTask, registeredBandsTask, undergroundArtistsTask, newReleasesArtistsTask);

        var localBands = await localBandsTask;
        var registeredBands = await registeredBandsTask;
        var undergroundArtists = await undergroundArtistsTask;
        var newReleaseArtists = await newReleasesArtistsTask;

        undergroundArtists.AddRange(newReleaseArtists);

        localBands = await _dataEnrichmentService.EnrichMultipleBands(localBands);
        undergroundArtists = await _dataEnrichmentService.EnrichMultipleBands(undergroundArtists);

        var similarBands = new List<BandModel>();
        var processedBandNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        localBands.ForEach(b => processedBandNames.Add(b.Name));
        registeredBands.ForEach(b => processedBandNames.Add(b.Name));
        undergroundArtists.ForEach(b => processedBandNames.Add(b.Name));

        var selectedArtists = topArtists.Take(5).Select(a => a.Name).ToList();
        var similarBandsDict = await _dataEnrichmentService.GetSimilarBandsForMultipleArtists(selectedArtists, 7, processedBandNames);

        foreach (var kvp in similarBandsDict)
        {
            foreach (var band in kvp.Value)
            {
                band.SimilarToArtistName = kvp.Key;
                similarBands.Add(band);
                processedBandNames.Add(band.Name);
            }
        }

        var selectedRegisteredBands = registeredBands.Take(5).Select(b => b.Name).ToList();
        var similarToRegisteredDict = await _dataEnrichmentService.GetSimilarBandsForMultipleArtists(selectedRegisteredBands, 3, processedBandNames, isRegisteredBandSimilar: true);

        foreach (var kvp in similarToRegisteredDict)
        {
            foreach (var band in kvp.Value)
            {
                band.SimilarToRegisteredBand = kvp.Key;
                similarBands.Add(band);
            }
        }

        var recommendedBands = new List<BandModel>();
        recommendedBands.AddRange(_scoringService.ScoreBands(localBands, genres, location, topArtists, isRegistered: false));
        recommendedBands.AddRange(_scoringService.ScoreBands(similarBands, genres, location, topArtists, isRegistered: false));
        recommendedBands.AddRange(_scoringService.ScoreBands(undergroundArtists, genres, location, topArtists, isRegistered: false, isFromSearch: true));
        recommendedBands.AddRange(_scoringService.ScoreBands(registeredBands, genres, location, topArtists, isRegistered: true));

        var now = DateTime.UtcNow;
        var keysToRemove = PreviouslyRecommendedBands
            .Where(kvp => (now - kvp.Value).TotalDays > MusicDiscoveryConstants.RecommendationCooldownDays)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            PreviouslyRecommendedBands.TryRemove(key, out _);
        }

        var finalRecommendations = _scoringService.ApplyDiversityAndExploration(recommendedBands, PreviouslyRecommendedBands);

        finalRecommendations = finalRecommendations
            .Where(band => !knownArtistNames.Contains(band.Name))
            .ToList();

        foreach (var band in finalRecommendations)
        {
            PreviouslyRecommendedBands[band.Name] = now;
        }

        return finalRecommendations;
    }
}
