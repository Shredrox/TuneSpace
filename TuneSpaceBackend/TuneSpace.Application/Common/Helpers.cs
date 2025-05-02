using System.Text;
using Newtonsoft.Json;
using TuneSpace.Core.DTOs.Requests.Spotify;

namespace TuneSpace.Application.Common;

public static class Helpers
{
    public static StringContent ToLowercaseJsonStringContent(CreatePlaylistRequest request)
    {
        var dictionary = new Dictionary<string, object>
        {
            ["name"] = request.Name,
            ["description"] = request.Description,
            ["public"] = request.Public
        };

        var jsonString = JsonConvert.SerializeObject(dictionary);

        return new StringContent(jsonString, Encoding.UTF8, "application/json");
    }

    public static string GenerateRandomString(int length)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string([.. Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)])]);
    }
}
