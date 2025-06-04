using System.Text.Json.Serialization;

namespace TuneSpace.Core.DTOs.Responses.Spotify;

public record UserTopSongsResponse(
    [property: JsonPropertyName("items")] List<SpotifyTopSongResponse> Items
);
