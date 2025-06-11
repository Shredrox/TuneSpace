using TuneSpace.Core.Interfaces.IClients;
using TuneSpace.Core.Interfaces.IServices;
using TuneSpace.Core.Models;
using TuneSpace.Core.Common;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using TuneSpace.Core.Entities;
using TuneSpace.Core.DTOs.Responses.Spotify;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace TuneSpace.Application.Services;

/// <summary>
/// Music Discovery Service - Provides intelligent music recommendations using multiple algorithms
///
/// Architecture Overview:
/// 1. User Data Collection - Gathers Spotify listening history and preferences
/// 2. Genre Analysis - Analyzes and enriches genres based on listening patterns
/// 3. Recommendation Sources - Fetches recommendations from various sources in parallel:
///    - Local bands (MusicBrainz)
///    - Registered bands (internal database)
///    - Underground/new release artists (Spotify search)
///    - AI recommendations (Enhanced AI or RAG)
///    - Collaborative filtering (user similarity)
/// 4. Data Processing - Enriches raw data and generates similar artist recommendations
/// 5. Scoring & Ranking - Applies adaptive scoring algorithms with confidence boosting
/// 6. Final Processing - Applies diversity algorithms, deduplication, and cooldown management
/// </summary>
internal class MusicDiscoveryService(
    ISpotifyService spotifyService,
    IArtistDiscoveryService artistDiscoveryService,
    IDataEnrichmentService dataEnrichmentService,
    IRecommendationScoringService scoringService,
    ICollaborativeFilteringService collaborativeFilteringService,
    IServiceProvider serviceProvider,
    ILogger<MusicDiscoveryService> logger) : IMusicDiscoveryService
{
    private static readonly ConcurrentDictionary<string, DateTime> PreviouslyRecommendedBands = new();
    private readonly ISpotifyService _spotifyService = spotifyService;
    private readonly IArtistDiscoveryService _artistDiscoveryService = artistDiscoveryService;
    private readonly IDataEnrichmentService _dataEnrichmentService = dataEnrichmentService;
    private readonly IRecommendationScoringService _scoringService = scoringService;
    private readonly ICollaborativeFilteringService _collaborativeFilteringService = collaborativeFilteringService;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<MusicDiscoveryService> _logger = logger;

    async Task<List<BandModel>> IMusicDiscoveryService.GetBandRecommendationsAsync(string spotifyAccessToken, string userId, List<string> genres, string location)
    {
        return await GetBandRecommendationsInternalAsync(spotifyAccessToken, userId, genres, location, useEnhancedAI: false);
    }

    async Task<List<BandModel>> IMusicDiscoveryService.GetEnhancedBandRecommendationsAsync(string spotifyAccessToken, string userId, List<string> genres, string location)
    {
        return await GetBandRecommendationsInternalAsync(spotifyAccessToken, userId, genres, location, useEnhancedAI: true);
    }

    /// <summary>
    /// Main recommendation engine - orchestrates the entire recommendation process
    ///
    /// Flow:
    /// 1. Collect user's Spotify data (recently played, followed artists, top artists)
    /// 2. Analyze and enrich genres based on listening history
    /// 3. Gather recommendations from all sources concurrently
    /// 4. Process and enrich raw recommendations with additional metadata
    /// 5. Apply adaptive scoring algorithms with confidence boosting
    /// 6. Apply final processing (diversity, deduplication, cooldown management)
    /// </summary>
    private async Task<List<BandModel>> GetBandRecommendationsInternalAsync(string spotifyAccessToken, string userId, List<string> genres, string location, bool useEnhancedAI)
    {
        var userData = await CollectUserDataAsync(spotifyAccessToken);

        var enrichedGenres = await AnalyzeAndEnrichGenresAsync(spotifyAccessToken, userData, genres);

        var recommendationSources = await GatherRecommendationSourcesAsync(
            spotifyAccessToken, userId, enrichedGenres, location, useEnhancedAI, userData);

        var processedRecommendations = await ProcessAndEnrichRecommendationsAsync(
            recommendationSources, userData.TopArtists, enrichedGenres);

        var scoredRecommendations = await ScoreAndRankRecommendationsAsync(
            processedRecommendations, enrichedGenres, userId, location, userData.TopArtists, useEnhancedAI);

        var finalRecommendations = await ApplyFinalProcessingAsync(scoredRecommendations, userData.KnownArtistNames);

        return finalRecommendations;
    }

    private static int ExtractUserCountFromDescription(string description)
    {
        if (string.IsNullOrEmpty(description))
        {
            return 0;
        }

        var match = Regex.Match(description, @"(\d+)\s+similar\s+users");
        return match.Success && int.TryParse(match.Groups[1].Value, out var count) ? count : 0;
    }

    /// <summary>
    /// Scores bands using adaptive weights if available, falls back to regular scoring
    /// </summary>
    private async Task<List<BandModel>> ScoreBandsAdaptivelyAsync(
        List<BandModel> bands,
        List<string> genres,
        string userId,
        string location,
        List<SpotifyArtistDTO> topArtists,
        bool isRegistered,
        bool isFromSearch = false)
    {
        try
        {
            var adaptiveScoringService = _serviceProvider.GetService<IAdaptiveRecommendationScoringService>();

            if (adaptiveScoringService != null)
            {
                return await adaptiveScoringService.ScoreBandsWithAdaptiveWeightsAsync(bands, genres, userId, location, topArtists, isRegistered, isFromSearch);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Adaptive scoring failed for user");
        }

        return _scoringService.ScoreBands(bands, genres, location, topArtists, isRegistered, isFromSearch);
    }

    public async Task TrackRecommendationInteractionAsync(string userId, string artistName, string interactionType, List<string> genres, double rating = 0.0)
    {
        try
        {
            var adaptiveLearningService = _serviceProvider.GetService<IAdaptiveLearningService>();

            if (adaptiveLearningService != null)
            {
                var feedback = new RecommendationFeedback
                {
                    UserId = userId,
                    BandName = artistName,
                    FeedbackType = ConvertInteractionTypeToFeedbackType(interactionType),
                    ExplicitRating = rating,
                    FeedbackAt = DateTime.UtcNow,
                    RecommendedAt = DateTime.UtcNow,
                    RecommendedGenres = genres
                };

                await adaptiveLearningService.ProcessRecommendationFeedbackAsync(feedback);

                _logger.LogInformation("Tracked recommendation interaction");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to track recommendation interaction");
        }
    }

    public async Task TrackBatchRecommendationInteractionsAsync(string userId, List<(string artistName, string interactionType, double rating, List<string> genres)> interactions)
    {
        try
        {
            var adaptiveLearningService = _serviceProvider.GetService<IAdaptiveLearningService>();

            if (adaptiveLearningService != null)
            {
                foreach (var (artistName, interactionType, rating, genres) in interactions)
                {
                    var feedback = new RecommendationFeedback
                    {
                        UserId = userId,
                        BandName = artistName,
                        FeedbackType = ConvertInteractionTypeToFeedbackType(interactionType),
                        ExplicitRating = rating,
                        RecommendedGenres = genres,
                        FeedbackAt = DateTime.UtcNow,
                        RecommendedAt = DateTime.UtcNow
                    };

                    await adaptiveLearningService.ProcessRecommendationFeedbackAsync(feedback);
                }

                _logger.LogInformation("Tracked batch recommendation interactions");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to track batch recommendation interactions");
        }
    }

    private static FeedbackType ConvertInteractionTypeToFeedbackType(string interactionType)
    {
        return interactionType?.ToLowerInvariant() switch
        {
            "like" or "thumbs_up" or "positive" => FeedbackType.Positive,
            "dislike" or "thumbs_down" or "negative" => FeedbackType.Negative,
            "rating" or "explicit" => FeedbackType.Explicit,
            "mixed" => FeedbackType.Mixed,
            _ => FeedbackType.None
        };
    }

    // Helper classes for organizing data
    private record UserData(
        List<RecentlyPlayedTrackDTO> RecentlyPlayedTracks,
        List<SpotifyArtistDTO> FollowedArtists,
        List<SpotifyArtistDTO> TopArtists,
        HashSet<string> KnownArtistNames);

    private record RecommendationSources(
        List<BandModel> LocalBands,
        List<BandModel> RegisteredBands,
        List<BandModel> UndergroundArtists,
        List<BandModel> NewReleaseArtists,
        (List<BandModel> Recommendations, double Confidence, string PromptUsed) AIResult,
        List<BandModel> CollaborativeRecommendations);

    private record ProcessedRecommendations(
        List<BandModel> LocalBands,
        List<BandModel> RegisteredBands,
        List<BandModel> UndergroundArtists,
        List<BandModel> SimilarBands,
        (List<BandModel> Recommendations, double Confidence, string PromptUsed) AIResult,
        List<BandModel> CollaborativeRecommendations);

    /// <summary>
    /// Step 1: Collect user data from Spotify
    /// </summary>
    private async Task<UserData> CollectUserDataAsync(string spotifyAccessToken)
    {
        var recentlyPlayedTracksTask = _spotifyService.GetUserRecentlyPlayedTracksAsync(spotifyAccessToken);
        var followedArtistsTask = _spotifyService.GetUserFollowedArtistsAsync(spotifyAccessToken);
        var topArtistsTask = _spotifyService.GetUserTopArtistsAsync(spotifyAccessToken);

        await Task.WhenAll(recentlyPlayedTracksTask, followedArtistsTask, topArtistsTask);

        var recentlyPlayedTracks = await recentlyPlayedTracksTask;
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

        return new UserData(recentlyPlayedTracks, followedArtists, topArtists, knownArtistNames);
    }

    /// <summary>
    /// Step 2: Analyze and enrich genres based on user's listening history
    /// </summary>
    private async Task<List<string>> AnalyzeAndEnrichGenresAsync(string spotifyAccessToken, UserData userData, List<string> baseGenres)
    {
        var followedArtistsGenres = userData.FollowedArtists
            .SelectMany(artist => artist.Genres ?? [])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var recentlyPlayedArtistIds = userData.RecentlyPlayedTracks
            .Select(track => track.ArtistId)
            .Where(id => !string.IsNullOrEmpty(id))
            .Distinct()
            .ToList();

        var recentArtistsWithGenres = await _artistDiscoveryService.GetArtistDetailsInBatchesAsync(spotifyAccessToken, recentlyPlayedArtistIds);

        var recentlyPlayedGenres = recentArtistsWithGenres?
            .SelectMany(artist => artist.Genres ?? [])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var combinedGenres = new List<string>();
        if (recentlyPlayedGenres != null)
        {
            combinedGenres.AddRange(recentlyPlayedGenres);
        }
        combinedGenres.AddRange(followedArtistsGenres.Where(g => !combinedGenres.Contains(g, StringComparer.OrdinalIgnoreCase)));

        return combinedGenres.Count > 0 ? combinedGenres : baseGenres;
    }

    /// <summary>
    /// Step 3: Gather recommendations from all sources in parallel
    /// </summary>
    private async Task<RecommendationSources> GatherRecommendationSourcesAsync(string spotifyAccessToken, string userId, List<string> genres, string location, bool useEnhancedAI, UserData userData)
    {
        var undergroundArtistsTask = _artistDiscoveryService.FindArtistsByQueryAsync(
            spotifyAccessToken, genres, "{genre}", MusicDiscoveryConstants.UndergroundArtistsToFetch);

        var newReleasesArtistsTask = _artistDiscoveryService.FindHipsterAndNewArtistsAsync(
            spotifyAccessToken, "{genre}", MusicDiscoveryConstants.UndergroundArtistsToFetch / 2);

        var localBandsTask = GetLocalBandsAsync(location, genres);
        var registeredBandsTask = GetRegisteredBandsAsync(genres, location);
        var aiRecommendationsTask = GetAIRecommendationsAsync(spotifyAccessToken, userId, userData.TopArtists, genres, location, useEnhancedAI);
        var collaborativeRecommendationsTask = GetCollaborativeRecommendationsAsync(userId, spotifyAccessToken);

        await Task.WhenAll(localBandsTask, registeredBandsTask, undergroundArtistsTask, newReleasesArtistsTask, aiRecommendationsTask, collaborativeRecommendationsTask);

        var localBands = await localBandsTask;
        var registeredBands = await registeredBandsTask;
        var undergroundArtists = await undergroundArtistsTask;
        var newReleaseArtists = await newReleasesArtistsTask;
        var aiResult = await aiRecommendationsTask;
        var collaborativeRecommendations = await collaborativeRecommendationsTask;

        undergroundArtists.AddRange(newReleaseArtists);

        return new RecommendationSources(localBands, registeredBands, undergroundArtists, newReleaseArtists, aiResult, collaborativeRecommendations);
    }

    /// <summary>
    /// Helper method to get local bands
    /// </summary>
    private async Task<List<BandModel>> GetLocalBandsAsync(string location, List<string> genres)
    {
        return await Task.Run(async () =>
        {
            using var scope = _serviceProvider.CreateScope();
            var musicBrainzClient = scope.ServiceProvider.GetRequiredService<IMusicBrainzClient>();
            return await musicBrainzClient.GetBandsByLocationAsync(location, 20, genres);
        });
    }

    /// <summary>
    /// Helper method to get registered bands
    /// </summary>
    private async Task<List<BandModel>> GetRegisteredBandsAsync(List<string> genres, string location)
    {
        return await Task.Run(async () =>
        {
            using var scope = _serviceProvider.CreateScope();
            var artistDiscoveryService = scope.ServiceProvider.GetRequiredService<IArtistDiscoveryService>();
            return await artistDiscoveryService.GetRegisteredBandsAsModelsAsync(genres, location);
        });
    }

    /// <summary>
    /// Helper method to get AI recommendations
    /// </summary>
    private async Task<(List<BandModel>, double, string)> GetAIRecommendationsAsync(
        string spotifyAccessToken, string userId, List<SpotifyArtistDTO> topArtists, List<string> genres, string location, bool useEnhancedAI)
    {
        return await Task.Run(async () =>
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var aiRecommendationService = scope.ServiceProvider.GetRequiredService<IAIRecommendationService>();
                var topArtistNames = topArtists.Take(10).Select(a => a.Name).ToList();

                if (useEnhancedAI)
                {
                    var enhancedResult = await aiRecommendationService.GetEnhancedRecommendationsWithConfidenceAsync(
                        spotifyAccessToken, userId, topArtistNames, genres, location, 15);

                    var bandModels = enhancedResult.Recommendations.Select(rec => new BandModel
                    {
                        Id = "",
                        Name = rec.ArtistName,
                        RelevanceScore = rec.ConfidenceScore,
                        DataSource = $"Enhanced AI (Confidence: {rec.ConfidenceScore:F2})",
                        Description = rec.Reasoning,
                        Genres = rec.MatchingGenres,
                        ExternalUrl = rec.ExternalUrl,
                        ExternalUrls = new Dictionary<string, string>
                        {
                            { "spotify", $"https://open.spotify.com/search/{Uri.EscapeDataString(rec.ArtistName)}" }
                        },
                        SimilarToArtistName = rec.Reasoning
                    }).ToList();

                    return (bandModels, enhancedResult.OverallConfidence, enhancedResult.PromptUsed);
                }
                else
                {
                    var ragRecs = await aiRecommendationService.GetRAGEnhancedRecommendationsAsync(
                        spotifyAccessToken, userId, topArtistNames, genres, location, 15);

                    return (ragRecs, 0.7, "RAG Enhanced Recommendations");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI recommendations failed for user {userId}, falling back to basic RAG: {ex.Message}");

                using var scope = _serviceProvider.CreateScope();
                var aiRecommendationService = scope.ServiceProvider.GetRequiredService<IAIRecommendationService>();
                var topArtistNames = topArtists.Take(10).Select(a => a.Name).ToList();
                var fallbackRecs = await aiRecommendationService.GetRAGEnhancedRecommendationsAsync(
                    spotifyAccessToken, userId, topArtistNames, genres, location, 15);

                return (fallbackRecs, 0.5, "Fallback recommendations due to AI service unavailability");
            }
        });
    }

    /// <summary>
    /// Helper method to get collaborative filtering recommendations
    /// </summary>
    private async Task<List<BandModel>> GetCollaborativeRecommendationsAsync(string userId, string spotifyAccessToken)
    {
        return await Task.Run(async () =>
        {
            try
            {
                await _collaborativeFilteringService.UpdateUserListeningBehaviorAsync(userId, spotifyAccessToken);
                var collabRecs = await _collaborativeFilteringService.GetCollaborativeRecommendationsAsync(userId, 20);

                return collabRecs.Select(rec => new BandModel
                {
                    Id = rec.ArtistId,
                    Name = rec.ArtistName,
                    RelevanceScore = Math.Min(1.0, rec.Score),
                    DataSource = "Collaborative Filtering",
                    Description = rec.ReasoningExplanation,
                    Genres = rec.CommonGenres,
                    ExternalUrls = new Dictionary<string, string>
                    {
                        { "spotify", $"https://open.spotify.com/artist/{rec.ArtistId}" }
                    },
                    SimilarToArtistName = rec.RecommendedBySimilarUsers.Count > 0 ?
                        $"Recommended by {rec.RecommendedBySimilarUsers.Count} similar users" : ""
                }).ToList();
            }
            catch
            {
                return [];
            }
        });
    }

    /// <summary>
    /// Step 4: Process and enrich the raw recommendations
    /// </summary>
    private async Task<ProcessedRecommendations> ProcessAndEnrichRecommendationsAsync(RecommendationSources sources, List<SpotifyArtistDTO> topArtists, List<string> genres)
    {
        var enrichedLocalBands = await _dataEnrichmentService.EnrichMultipleBandsAsync(sources.LocalBands);
        var enrichedUndergroundArtists = await _dataEnrichmentService.EnrichMultipleBandsAsync(sources.UndergroundArtists);

        var similarBands = await GenerateSimilarBandsAsync(topArtists, sources.RegisteredBands, sources, enrichedLocalBands, enrichedUndergroundArtists);

        return new ProcessedRecommendations(
            enrichedLocalBands,
            sources.RegisteredBands,
            enrichedUndergroundArtists,
            similarBands,
            sources.AIResult,
            sources.CollaborativeRecommendations);
    }

    /// <summary>
    /// Helper method to generate similar bands
    /// </summary>
    private async Task<List<BandModel>> GenerateSimilarBandsAsync(
        List<SpotifyArtistDTO> topArtists,
        List<BandModel> registeredBands,
        RecommendationSources sources,
        List<BandModel> enrichedLocalBands,
        List<BandModel> enrichedUndergroundArtists)
    {
        var similarBands = new List<BandModel>();
        var processedBandNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        enrichedLocalBands.ForEach(b => processedBandNames.Add(b.Name));
        registeredBands.ForEach(b => processedBandNames.Add(b.Name));
        enrichedUndergroundArtists.ForEach(b => processedBandNames.Add(b.Name));
        sources.CollaborativeRecommendations.ForEach(b => processedBandNames.Add(b.Name));
        sources.AIResult.Recommendations.ForEach(b => processedBandNames.Add(b.Name));

        var selectedArtists = topArtists.Take(5).Select(a => a.Name).ToList();
        var similarBandsDict = await _dataEnrichmentService.GetSimilarBandsForMultipleArtistsAsync(selectedArtists, 7, processedBandNames);

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
        var similarToRegisteredDict = await _dataEnrichmentService.GetSimilarBandsForMultipleArtistsAsync(
            selectedRegisteredBands, 3, processedBandNames, isRegisteredBandSimilar: true);

        foreach (var kvp in similarToRegisteredDict)
        {
            foreach (var band in kvp.Value)
            {
                band.SimilarToRegisteredBand = kvp.Key;
                similarBands.Add(band);
            }
        }

        return similarBands;
    }

    /// <summary>
    /// Step 5: Score and rank all recommendations
    /// </summary>
    private async Task<List<BandModel>> ScoreAndRankRecommendationsAsync(
        ProcessedRecommendations processed, List<string> genres, string userId, string location,
        List<SpotifyArtistDTO> topArtists, bool useEnhancedAI)
    {
        var recommendedBands = new List<BandModel>();

        var localScoredBands = await ScoreBandsAdaptivelyAsync(processed.LocalBands, genres, userId, location, topArtists, isRegistered: false);
        var similarScoredBands = await ScoreBandsAdaptivelyAsync(processed.SimilarBands, genres, userId, location, topArtists, isRegistered: false);
        var undergroundScoredBands = await ScoreBandsAdaptivelyAsync(processed.UndergroundArtists, genres, userId, location, topArtists, isRegistered: false, isFromSearch: true);
        var registeredScoredBands = await ScoreBandsAdaptivelyAsync(processed.RegisteredBands, genres, userId, location, topArtists, isRegistered: true);

        recommendedBands.AddRange(localScoredBands);
        recommendedBands.AddRange(similarScoredBands);
        recommendedBands.AddRange(undergroundScoredBands);
        recommendedBands.AddRange(registeredScoredBands);

        var aiScoredBands = await ApplyAIScoringAsync(processed.AIResult, genres, userId, location, topArtists, useEnhancedAI);
        recommendedBands.AddRange(aiScoredBands);

        var collaborativeScoredBands = ApplyCollaborativeScoring(processed.CollaborativeRecommendations, genres, location, topArtists);
        recommendedBands.AddRange(collaborativeScoredBands);

        return recommendedBands;
    }

    /// <summary>
    /// Apply specialized scoring for AI recommendations
    /// </summary>
    private async Task<List<BandModel>> ApplyAIScoringAsync(
        (List<BandModel> Recommendations, double Confidence, string PromptUsed) aiResult,
        List<string> genres, string userId, string location, List<SpotifyArtistDTO> topArtists, bool useEnhancedAI)
    {
        var aiScoredBands = await ScoreBandsAdaptivelyAsync(aiResult.Recommendations, genres, userId, location, topArtists, isRegistered: false, isFromSearch: true);

        foreach (var band in aiScoredBands)
        {
            if (useEnhancedAI)
            {
                band.RelevanceScore += 0.5;
                band.DataSource = $"Enhanced AI + Adaptive Learning (Confidence: {aiResult.Confidence:F2})";
            }
            else
            {
                band.RelevanceScore += 0.3;
                band.DataSource = $"RAG AI (Confidence: {aiResult.Confidence:F2})";
            }

            if (!string.IsNullOrEmpty(location) &&
                band.Location?.Contains(location, StringComparison.OrdinalIgnoreCase) == true)
            {
                band.RelevanceScore += 0.2;
            }

            var genreOverlap = band.Genres.Intersect(genres, StringComparer.OrdinalIgnoreCase).Count();
            if (genreOverlap > 0 && genreOverlap < genres.Count)
            {
                band.RelevanceScore += 0.1;
            }

            band.RelevanceScore += aiResult.Confidence * 0.2;
        }

        return aiScoredBands;
    }

    /// <summary>
    /// Apply specialized scoring for collaborative filtering recommendations
    /// </summary>
    private List<BandModel> ApplyCollaborativeScoring(
        List<BandModel> collaborativeRecommendations, List<string> genres, string location, List<SpotifyArtistDTO> topArtists)
    {
        var collaborativeScoredBands = _scoringService.ScoreBands(collaborativeRecommendations, genres, location, topArtists, isRegistered: false, isFromSearch: false);

        foreach (var band in collaborativeScoredBands)
        {
            band.RelevanceScore += 0.35;
            band.DataSource = "Collaborative Filtering";

            if (band.SimilarToArtistName?.Contains("users") == true)
            {
                var userCount = ExtractUserCountFromDescription(band.SimilarToArtistName);
                if (userCount >= 3)
                {
                    band.RelevanceScore += 0.15;
                }
            }

            if (!string.IsNullOrEmpty(location) &&
                band.Location?.Contains(location, StringComparison.OrdinalIgnoreCase) == true)
            {
                band.RelevanceScore += 0.1;
            }
        }

        return collaborativeScoredBands;
    }

    /// <summary>
    /// Step 6: Apply final processing (diversity, deduplication, cooldown)
    /// </summary>
    private Task<List<BandModel>> ApplyFinalProcessingAsync(List<BandModel> scoredRecommendations, HashSet<string> knownArtistNames)
    {
        var now = DateTime.UtcNow;
        var keysToRemove = PreviouslyRecommendedBands
            .Where(kvp => (now - kvp.Value).TotalDays > MusicDiscoveryConstants.RecommendationCooldownDays)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            PreviouslyRecommendedBands.TryRemove(key, out _);
        }

        var finalRecommendations = _scoringService.ApplyDiversityAndExploration(scoredRecommendations, PreviouslyRecommendedBands);

        finalRecommendations = [.. finalRecommendations.Where(band => !knownArtistNames.Contains(band.Name))];

        foreach (var band in finalRecommendations)
        {
            PreviouslyRecommendedBands[band.Name] = now;
        }

        return Task.FromResult(finalRecommendations);
    }
}
