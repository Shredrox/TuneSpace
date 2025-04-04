namespace TuneSpace.Core.Interfaces.IClients;

public interface IOllamaClient
{
    Task<string> Prompt(string location, List<string> genres);
}
