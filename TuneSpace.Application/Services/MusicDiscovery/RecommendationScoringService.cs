using System.Collections.Concurrent;
using TuneSpace.Core.Common;
using TuneSpace.Core.DTOs.Responses.Spotify;
using TuneSpace.Core.Interfaces.IServices;
using TuneSpace.Core.Models;

namespace TuneSpace.Application.Services.MusicDiscovery;

internal class RecommendationScoringService : IRecommendationScoringService
{
    private static readonly ConcurrentDictionary<string, DateTime> _registeredBandDates = new();
    private readonly Lock _recommendationLock = new();

    List<BandModel> IRecommendationScoringService.ScoreBands(
        List<BandModel> bands,
        List<string> userGenres,
        string location,
        List<TopArtistDTO> topArtists,
        bool isRegistered,
        bool isFromSearch)
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

                score += genreMatchCount * MusicDiscoveryConstants.GenreMatchScore;
            }

            if (!string.IsNullOrEmpty(band.Location) && band.Location.Equals(location, StringComparison.OrdinalIgnoreCase))
            {
                score += MusicDiscoveryConstants.LocationMatchBonus;
            }

            score += (1.0 - (band.Listeners / 1000000.0)) * MusicDiscoveryConstants.ListenerScoreFactor;

            foreach (var artist in topArtists)
            {
                if (band.SimilarArtists.Any(sa => sa.Equals(artist.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    score += MusicDiscoveryConstants.SimilarArtistBonus;
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
                if (random.NextDouble() < 0.3 && !_registeredBandDates.ContainsKey(band.Name))
                {
                    _registeredBandDates[band.Name] = DateTime.UtcNow.AddDays(-random.Next(1, 29));
                }

                if (_registeredBandDates.TryGetValue(band.Name, out DateTime registrationDate))
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

                    score += genreMatchCount * MusicDiscoveryConstants.ExtraGenreMatchScore;
                }

                if (band.IsNewRelease)
                {
                    score += MusicDiscoveryConstants.NewReleaseBonus;
                }

                if (band.SearchTags?.Contains("hipster") == true)
                {
                    score += MusicDiscoveryConstants.HipsterTagBonus;
                }
            }

            band.RelevanceScore = Math.Min(score, 1.0);
        }

        return bands;
    }

    List<BandModel> IRecommendationScoringService.ApplyDiversityAndExploration(List<BandModel> recommendedBands, ConcurrentDictionary<string, DateTime> previouslyRecommendedBands)
    {
        var now = DateTime.UtcNow;
        var result = new List<BandModel>();
        var genreCounters = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        lock (_recommendationLock)
        {
            var registeredBands = recommendedBands.Where(b => b.IsRegistered).ToList();
            var externalBands = recommendedBands.Where(b => !b.IsRegistered).ToList();

            // First pass - exclude recently recommended bands unless they're highly relevant
            var candidateRegisteredBands = registeredBands
                .Where(b => !previouslyRecommendedBands.ContainsKey(b.Name) ||
                        b.RelevanceScore > 0.7)
                .OrderByDescending(b => b.RelevanceScore)
                .ToList();

            var candidateExternalBands = externalBands
                .Where(b => !previouslyRecommendedBands.ContainsKey(b.Name) ||
                        b.RelevanceScore > 0.8)
                .OrderByDescending(b => b.RelevanceScore)
                .ToList();

            // Second pass - calculate diversity scores
            CalculateDiversityScores(candidateRegisteredBands, genreCounters, now, previouslyRecommendedBands);
            CalculateDiversityScores(candidateExternalBands, genreCounters, now, previouslyRecommendedBands);

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
                    if (!genreCounters.TryGetValue(genre, out int value))
                    {
                        value = 0;
                        genreCounters[genre] = value;
                    }
                    genreCounters[genre] = ++value;
                }
            }

            CalculateDiversityScores(candidateExternalBands, genreCounters, now, previouslyRecommendedBands);

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
        }

        return result;
    }

    private void CalculateDiversityScores(List<BandModel> bands, Dictionary<string, int> genreCounters, DateTime now, ConcurrentDictionary<string, DateTime> previouslyRecommendedBands)
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

            if (previouslyRecommendedBands.TryGetValue(band.Name, out DateTime lastRecommended))
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
}
