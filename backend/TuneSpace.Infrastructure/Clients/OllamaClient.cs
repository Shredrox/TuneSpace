using System.Text;
using System.Text.Json;
using TuneSpace.Core.Interfaces.IClients;

namespace TuneSpace.Infrastructure.Clients;

internal class OllamaClient(HttpClient httpClient) : IOllamaClient
{
    private readonly HttpClient _httpClient = httpClient;

    async Task<string> IOllamaClient.Prompt(string location, List<string> genres)
    {
        var requestData = new
        {
            model = "band-recommender",
            prompt = $"The user is from {location} and prefers these genres {string.Join(",", genres)}. Suggest underground and lesser known bands from their location that match their taste. (give them in a list without descriptions) and don't say anything else",
            stream = false
        };

        string json = JsonSerializer.Serialize(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("http://localhost:11434/api/generate", content);
        response.EnsureSuccessStatusCode();

        string result = await response.Content.ReadAsStringAsync();
        return result;
    }
}
