using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using TuneSpace.Core.Interfaces.IClients;
using TuneSpace.Core.Models;

namespace TuneSpace.Infrastructure.Clients;

internal class LastFmClient(HttpClient httpClient, IConfiguration configuration) : ILastFmClient
{
    private readonly string _apiKey = configuration["ExternalApis:LastFm:ApiKey"]
            ?? throw new ArgumentNullException(nameof(configuration),
                "LastFm API key is not configured in application settings");
    private const string BaseUrl = "https://ws.audioscrobbler.com/2.0/";

    async Task<BandModel> ILastFmClient.GetBandDataAsync(string bandName)
    {
        var apiUrl = $"{BaseUrl}?method=artist.getInfo&artist={bandName}&api_key={_apiKey}&format=json";
        var response = await httpClient.GetStringAsync(apiUrl);
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
                    imageUrl = img["#text"].ToString();
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
            .ToList() ?? new List<string>();

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
        var apiUrl = $"{BaseUrl}?method=artist.getSimilar&artist={bandName}&api_key={_apiKey}&limit={limit}&format=json";
        var response = await httpClient.GetStringAsync(apiUrl);
        var similarBands = JObject.Parse(response);

        var artists = similarBands["similarartists"]?["artist"]?.ToObject<JArray>();
        if (artists == null)
        {
            return new List<string>();
        }

        return artists
            .Select(artist => artist["name"]?.ToString() ?? "")
            .Where(name => !string.IsNullOrEmpty(name))
            .ToList();
    }

    async Task<List<BandModel>> ILastFmClient.EnrichBandData(List<BandModel> bands)
    {
        foreach (var band in bands)
        {
            try
            {
                var bandData = await ((ILastFmClient)this).GetBandDataAsync(band.Name);

                band.Listeners = bandData.Listeners;
                band.PlayCount = bandData.PlayCount;
                band.ImageUrl = bandData.ImageUrl;

                if (band.Genres.Count == 0)
                {
                    band.Genres = bandData.Genres;
                }
                else if (bandData.Genres.Count > 0)
                {
                    band.Genres.AddRange(bandData.Genres.Except(band.Genres));
                }

                band.SimilarArtists = await ((ILastFmClient)this).GetSimilarBandsAsync(band.Name, 5);
            }
            catch (Exception)
            {
                continue;
            }
        }

        return bands;
    }
}
