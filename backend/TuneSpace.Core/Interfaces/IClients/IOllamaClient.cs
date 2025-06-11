namespace TuneSpace.Core.Interfaces.IClients;

/// <summary>
/// Provides methods for interacting with the Ollama AI service for music recommendations.
/// </summary>
public interface IOllamaClient
{
    /// <summary>
    /// Sends a prompt to the Ollama service to generate music recommendations based on location and genres.
    /// </summary>
    /// <param name="location">The geographical location to consider for music recommendations.</param>
    /// <param name="genres">A list of music genres to include in the recommendation criteria.</param>
    /// <returns>A string containing the AI-generated music recommendations.</returns>
    Task<string> Prompt(string location, List<string> genres);

    /// <summary>
    /// Sends an enhanced prompt to the Ollama service for RAG-based music recommendations.
    /// </summary>
    /// <param name="enhancedPrompt">The full enhanced prompt with context from vector search and user preferences.</param>
    /// <returns>A string containing the AI-generated music recommendations.</returns>
    Task<string> PromptWithContext(string enhancedPrompt);
}
