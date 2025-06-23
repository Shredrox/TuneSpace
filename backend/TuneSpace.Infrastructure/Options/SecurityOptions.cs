namespace TuneSpace.Infrastructure.Options;

public class SecurityOptions
{
    public string[] AllowedRedirectHosts { get; set; } = ["localhost", "127.0.0.1"];
}
