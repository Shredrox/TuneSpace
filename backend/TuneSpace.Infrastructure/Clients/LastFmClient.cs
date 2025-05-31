using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using TuneSpace.Core.Interfaces.IClients;
using TuneSpace.Core.Models;
using TuneSpace.Infrastructure.Options;

namespace TuneSpace.Infrastructure.Clients;

internal class LastFmClient(
    HttpClient httpClient,
    IOptions<LastFmOptions> lastFmOptions) : ILastFmClient
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly LastFmOptions _lastFmOptions = lastFmOptions.Value;

    private const string BaseUrl = "https://ws.audioscrobbler.com/2.0/";

    async Task<BandModel> ILastFmClient.GetBandDataAsync(string bandName)
    {
        var apiUrl = $"{BaseUrl}?method=artist.getInfo&artist={bandName}&api_key={_lastFmOptions.ApiKey}&format=json";
        var response = await _httpClient.GetStringAsync(apiUrl);
        var bandData = JObject.Parse(response);

        var artist = bandData["artist"];

        if (artist == null)
        {
            return new BandModel { Name = bandName };
        }

        var name = artist["name"]?.ToString() ?? bandName;
        var listeners = int.TryParse(artist["listeners"]?.ToString(), out var l) ? l : 0;
        var playCount = int.TryParse(artist["stats"]?["playcount"]?.ToString(), out var pc) ? pc : 0;

        string imageUrl = "";
        var images = artist["image"]?.ToObject<JArray>();
        if (images != null)
        {
            var sizeOrder = new[] { "extralarge", "large", "medium", "small" };

            foreach (var size in sizeOrder)
            {
                var img = images.FirstOrDefault(i => i["size"]?.ToString() == size);
                if (img != null && !string.IsNullOrWhiteSpace(img["#text"]?.ToString()))
                {
                    imageUrl = img["#text"]?.ToString() ?? "";
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                imageUrl = images
                    .Select(img => img["#text"]?.ToString())
                    .FirstOrDefault(url => !string.IsNullOrWhiteSpace(url)) ?? "";
            }
        }

        var tags = artist["tags"]?["tag"]?.ToObject<JArray>();
        var genres = tags?.Select(t => t["name"]?.ToString() ?? "")
            .Where(t => !string.IsNullOrEmpty(t))
            .ToList() ?? [];

        return new BandModel
        {
            Name = name,
            Listeners = listeners,
            PlayCount = playCount,
            ImageUrl = imageUrl,
            Genres = genres
        };
    }

    async Task<List<string>> ILastFmClient.GetSimilarBandsAsync(string bandName, int limit)
    {
        var apiUrl = $"{BaseUrl}?method=artist.getSimilar&artist={bandName}&api_key={_lastFmOptions.ApiKey}&limit={limit}&format=json";
        var response = await _httpClient.GetStringAsync(apiUrl);
        var similarBands = JObject.Parse(response);

        var artists = similarBands["similarartists"]?["artist"]?.ToObject<JArray>();
        if (artists is null)
        {
            return [];
        }

        return [.. artists
            .Select(artist => artist["name"]?.ToString() ?? "")
            .Where(name => !string.IsNullOrEmpty(name))];
    }
}
