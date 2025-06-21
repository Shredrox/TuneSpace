using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TuneSpace.Core.Common;
using TuneSpace.Core.DTOs.Responses.Spotify;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IServices;
using TuneSpace.Core.Models;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Application.Services.MusicDiscovery;

internal class AdaptiveRecommendationScoringService(
    ILogger<AdaptiveRecommendationScoringService> logger,
    TuneSpaceDbContext context,
    IRecommendationScoringService baseScoringService) : IAdaptiveRecommendationScoringService
{
    private readonly ILogger<AdaptiveRecommendationScoringService> _logger = logger;
    private readonly TuneSpaceDbContext _context = context;
    private readonly IRecommendationScoringService _baseScoringService = baseScoringService;
    private static readonly ConcurrentDictionary<string, DateTime> _registeredBandDates = new();
    private readonly Lock _recommendationLock = new();

    async Task<List<BandModel>> IAdaptiveRecommendationScoringService.ScoreBandsWithAdaptiveWeightsAsync(
        List<BandModel> bands,
        List<string> userGenres,
        string userId,
        string location,
        List<SpotifyArtistDTO> topArtists,
        bool isRegistered,
        bool isFromSearch)
    {
        try
        {
            var weights = await GetOrCreateUserWeightsAsync(userId);

            foreach (var band in bands)
            {
                var scoringFactors = await ((IAdaptiveRecommendationScoringService)this).GetScoringFactorsForBand(band, userGenres, location, topArtists, isRegistered, isFromSearch, weights);

                double adaptiveScore = CalculateAdaptiveScore(scoringFactors, weights);

                band.RelevanceScore = Math.Min(adaptiveScore, 1.0);
            }

            _logger.LogInformation("Scored {BandCount} bands using adaptive weights for user {UserId}", bands.Count, userId);

            return bands;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in adaptive scoring for user {UserId}, falling back to base scoring", userId);

            return _baseScoringService.ScoreBands(bands, userGenres, location, topArtists, isRegistered, isFromSearch);
        }
    }

    async Task<Dictionary<string, double>> IAdaptiveRecommendationScoringService.GetScoringFactorsForBand(
        BandModel band,
        List<string> userGenres,
        string location,
        List<SpotifyArtistDTO> topArtists,
        bool isRegistered,
        bool isFromSearch,
        DynamicScoringWeights weights)
    {
        var factors = new Dictionary<string, double>();

        if (userGenres.Count > 0 && band.Genres.Count > 0)
        {
            var genreMatchCount = band.Genres
                .Count(g => userGenres.Any(ug =>
                    ug.Equals(g, StringComparison.OrdinalIgnoreCase) ||
                    g.Contains(ug, StringComparison.OrdinalIgnoreCase) ||
                    ug.Contains(g, StringComparison.OrdinalIgnoreCase)));

            factors["GenreMatch"] = genreMatchCount > 0 ? (double)genreMatchCount / Math.Max(band.Genres.Count, userGenres.Count) : 0.0;
        }
        else
        {
            factors["GenreMatch"] = 0.0;
        }

        factors["LocationMatch"] = !string.IsNullOrEmpty(band.Location) &&
            band.Location.Equals(location, StringComparison.OrdinalIgnoreCase) ? 1.0 : 0.0;

        factors["ListenerScore"] = band.Listeners > 0 ?
            Math.Max(0.0, 1.0 - (band.Listeners / 1000000.0)) : 0.5;

        factors["SimilarArtist"] = topArtists.Any(artist =>
            band.SimilarArtists.Any(sa => sa.Equals(artist.Name, StringComparison.OrdinalIgnoreCase))) ? 1.0 : 0.0;

        factors["UndergroundBand"] = band.IsLesserKnown ? 1.0 : 0.0;

        factors["NewRelease"] = band.IsNewRelease ? 1.0 : 0.0;

        factors["RegisteredBand"] = isRegistered ? 1.0 : 0.0;

        if (isFromSearch && band.IsLesserKnown)
        {
            factors["SearchUndergroundBonus"] = band.Popularity <= 20 ? 1.5 : 1.0;
        }
        else
        {
            factors["SearchUndergroundBonus"] = 0.0;
        }

        if (isRegistered)
        {
            factors["NewRegistrationBonus"] = await CalculateNewRegistrationBonusAsync(band.Name);
        }
        else
        {
            factors["NewRegistrationBonus"] = 0.0;
        }

        factors["HipsterTag"] = band.SearchTags?.Contains("hipster") == true ? 1.0 : 0.0;

        return factors;
    }

    async Task IAdaptiveRecommendationScoringService.RecordRecommendationInteractionAsync(
        string userId,
        string bandId,
        string bandName,
        List<string> genres,
        Dictionary<string, double> scoringFactors,
        double initialScore)
    {
        try
        {
            var feedback = new RecommendationFeedback
            {
                UserId = Guid.Parse(userId),
                BandId = bandId,
                BandName = bandName,
                RecommendedGenres = genres,
                InitialScore = initialScore,
                ScoringFactors = scoringFactors,
                RecommendedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            _context.RecommendationFeedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Recorded recommendation interaction for user {UserId} with band {BandName}",
                userId, bandName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record recommendation interaction for user {UserId}", userId);
        }
    }

    public List<BandModel> ScoreBands(
        List<BandModel> bands,
        List<string> userGenres,
        string location,
        List<SpotifyArtistDTO> topArtists,
        bool isRegistered,
        bool isFromSearch = false)
    {
        return _baseScoringService.ScoreBands(bands, userGenres, location, topArtists, isRegistered, isFromSearch);
    }

    public List<BandModel> ApplyDiversityAndExploration(
        List<BandModel> recommendedBands,
        ConcurrentDictionary<string, DateTime> previouslyRecommendedBands)
    {
        return _baseScoringService.ApplyDiversityAndExploration(recommendedBands, previouslyRecommendedBands);
    }

    private async Task<DynamicScoringWeights> GetOrCreateUserWeightsAsync(string userId)
    {
        var weights = await _context.DynamicScoringWeights
            .FirstOrDefaultAsync(w => w.UserId.ToString() == userId);

        if (weights == null)
        {
            weights = new DynamicScoringWeights
            {
                UserId = Guid.Parse(userId),
                GenreMatchWeight = MusicDiscoveryConstants.GenreMatchScore,
                LocationMatchWeight = MusicDiscoveryConstants.LocationMatchBonus,
                ListenerScoreWeight = MusicDiscoveryConstants.ListenerScoreFactor,
                SimilarArtistWeight = MusicDiscoveryConstants.SimilarArtistBonus,
                UndergroundBandWeight = MusicDiscoveryConstants.UndergroundBandBonus,
                NewReleaseWeight = MusicDiscoveryConstants.NewReleaseBonus,
                RegisteredBandWeight = MusicDiscoveryConstants.RegisteredBandBonus,
                ExplorationFactor = MusicDiscoveryConstants.ExplorationFactor,
                DiversityFactor = MusicDiscoveryConstants.DiversityFactor,
                LearningRate = 0.01,
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                LastAdaptation = DateTime.UtcNow
            };

            _context.DynamicScoringWeights.Add(weights);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created new dynamic scoring weights for user {UserId}", userId);
        }

        return weights;
    }

    private static double CalculateAdaptiveScore(Dictionary<string, double> factors, DynamicScoringWeights weights)
    {
        double score = 0.0;

        if (factors.TryGetValue("GenreMatch", out var genreMatch))
            score += genreMatch * weights.GenreMatchWeight;

        if (factors.TryGetValue("LocationMatch", out var locationMatch))
            score += locationMatch * weights.LocationMatchWeight;

        if (factors.TryGetValue("ListenerScore", out var listenerScore))
            score += listenerScore * weights.ListenerScoreWeight;

        if (factors.TryGetValue("SimilarArtist", out var similarArtist))
            score += similarArtist * weights.SimilarArtistWeight;

        if (factors.TryGetValue("UndergroundBand", out var undergroundBand))
            score += undergroundBand * weights.UndergroundBandWeight;

        if (factors.TryGetValue("NewRelease", out var newRelease))
            score += newRelease * weights.NewReleaseWeight;

        if (factors.TryGetValue("RegisteredBand", out var registeredBand))
            score += registeredBand * weights.RegisteredBandWeight;

        if (factors.TryGetValue("SearchUndergroundBonus", out var searchBonus))
            score += searchBonus * (weights.UndergroundBandWeight * 0.5);

        if (factors.TryGetValue("NewRegistrationBonus", out var newRegBonus))
            score += newRegBonus;

        if (factors.TryGetValue("HipsterTag", out var hipsterTag))
            score += hipsterTag * MusicDiscoveryConstants.HipsterTagBonus;

        return score;
    }

    private Task<double> CalculateNewRegistrationBonusAsync(string bandName)
    {
        try
        {
            lock (_recommendationLock)
            {
                var random = new Random();
                if (random.NextDouble() < 0.3 && !_registeredBandDates.ContainsKey(bandName))
                {
                    _registeredBandDates[bandName] = DateTime.UtcNow.AddDays(-random.Next(1, 29));
                }

                if (_registeredBandDates.TryGetValue(bandName, out DateTime registrationDate))
                {
                    double daysSinceRegistration = (DateTime.UtcNow - registrationDate).TotalDays;
                    if (daysSinceRegistration <= 30)
                    {
                        return Task.FromResult(MusicDiscoveryConstants.NewRegistrationBonus);
                    }
                }

                return Task.FromResult(0.0);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error calculating new registration bonus for band {BandName}", bandName);
            return Task.FromResult(0.0);
        }
    }
}
