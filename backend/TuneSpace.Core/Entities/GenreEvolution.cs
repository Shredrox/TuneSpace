using System.ComponentModel.DataAnnotations;

namespace TuneSpace.Core.Entities;

public class GenreEvolution
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string UserId { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;

    public double CurrentPreference { get; set; } = 0.0;
    public double PreviousPreference { get; set; } = 0.0;
    public double PreferenceChange { get; set; } = 0.0;
    public double PreferenceVelocity { get; set; } = 0.0;

    public DateTime FirstEncountered { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public int EncounterCount { get; set; } = 1;

    public Dictionary<int, double> MonthlyPreferences { get; set; } = [];
    public Dictionary<DayOfWeek, double> WeeklyPreferences { get; set; } = [];

    public GenreLifecycleStage LifecycleStage { get; set; } = GenreLifecycleStage.Discovery;

    public double PredictionConfidence { get; set; } = 0.5;
    public DateTime NextUpdatePredicted { get; set; } = DateTime.UtcNow.AddDays(7);

    public double SocialInfluence { get; set; } = 0.0;
    public double TrendInfluence { get; set; } = 0.0;
    public double SeasonalInfluence { get; set; } = 0.0;
}

public enum GenreLifecycleStage
{
    Discovery,
    Growth,
    Maturity,
    Decline,
    Rediscovery
}
