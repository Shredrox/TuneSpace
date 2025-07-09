namespace TuneSpace.Infrastructure.Options;

public class DatabaseOptions
{
    public string DefaultConnection { get; set; } = null!;
    public bool AutoMigrate { get; set; } = true;
}
