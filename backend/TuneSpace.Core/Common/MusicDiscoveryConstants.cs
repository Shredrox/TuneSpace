namespace TuneSpace.Core.Common;

/// <summary>
/// Constants used by the MusicDiscoveryService
/// </summary>
public static class MusicDiscoveryConstants
{
    public const int MaxRecommendations = 30;
    public const double RegisteredBandBonus = 0.5;
    public const double DiversityFactor = 0.15;
    public const double ExplorationFactor = 0.1;
    public const double UndergroundBandBonus = 0.25;
    public const double NewRegistrationBonus = 0.2;
    public const double MinRegisteredBandPercentage = 0.35;
    public const int RecommendationCooldownDays = 7;
    public const int MaxPopularityForUnderground = 30;
    public const int UndergroundArtistsToFetch = 25;
    public const int UndergroundListenersThreshold = 10000;

    public const double GenreMatchScore = 0.3;
    public const double LocationMatchBonus = 0.3;
    public const double ListenerScoreFactor = 0.2;
    public const double SimilarArtistBonus = 0.2;
    public const double ExtraGenreMatchScore = 0.15;
    public const double NewReleaseBonus = 0.2;
    public const double HipsterTagBonus = 0.1;
}
