namespace TuneSpace.Core.Common;

/// <summary>
/// Constants used by the MusicDiscoveryService
/// </summary>
public static class MusicDiscoveryConstants
{
    public const int MaxRecommendations = 20;
    public const double RegisteredBandBonus = 0.5;
    public const double DiversityFactor = 0.15;
    public const double ExplorationFactor = 0.1;
    public const double UndergroundBandBonus = 0.25;
    public const double NewRegistrationBonus = 0.2;
    public const double MinRegisteredBandPercentage = 0.35;
    public const int RecommendationCooldownDays = 14;
    public const int MaxPopularityForUnderground = 40;
    public const int UndergroundArtistsToFetch = 15;
}
