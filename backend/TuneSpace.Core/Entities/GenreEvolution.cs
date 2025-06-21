namespace TuneSpace.Core.Entities;

public class GenreEvolution
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
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

    public User User { get; set; } = null!;
}

public enum GenreLifecycleStage
{
    Discovery,
    Growth,
    Maturity,
    Decline,
    Rediscovery
}
