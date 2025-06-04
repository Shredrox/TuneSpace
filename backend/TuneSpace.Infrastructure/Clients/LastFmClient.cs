using Microsoft.Extensions.Options;
using System.Text.Json;
using TuneSpace.Core.Interfaces.IClients;
using TuneSpace.Core.Models;
using TuneSpace.Core.DTOs.Responses.LastFm;
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

        var bandData = JsonSerializer.Deserialize<LastFmArtistInfoResponse>(response);
        var artist = bandData?.Artist;

        if (artist == null)
        {
            return new BandModel { Name = bandName };
        }

        var name = artist.Name ?? bandName;
        var listeners = int.TryParse(artist.Listeners, out var l) ? l : 0;
        var playCount = int.TryParse(artist.Stats?.PlayCount, out var pc) ? pc : 0;

        string imageUrl = "";
        var images = artist.Images;
        if (images != null)
        {
            var sizeOrder = new[] { "extralarge", "large", "medium", "small" };

            foreach (var size in sizeOrder)
            {
                var img = images.FirstOrDefault(i => i.Size == size);
                if (img != null && !string.IsNullOrWhiteSpace(img.Text))
                {
                    imageUrl = img.Text;
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                imageUrl = images
                    .Select(img => img.Text)
                    .FirstOrDefault(url => !string.IsNullOrWhiteSpace(url)) ?? "";
            }
        }

        var tags = artist.Tags?.Tag;
        var genres = tags?.Select(t => t.Name ?? "")
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

        var similarBands = JsonSerializer.Deserialize<LastFmSimilarArtistsResponse>(response);
        var artists = similarBands?.SimilarArtists?.Artists;

        if (artists is null)
        {
            return [];
        }

        return [.. artists
            .Select(artist => artist.Name ?? "")
            .Where(name => !string.IsNullOrEmpty(name))];
    }
}
