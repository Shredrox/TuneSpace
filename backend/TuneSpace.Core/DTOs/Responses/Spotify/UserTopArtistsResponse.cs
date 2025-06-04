using System.Text.Json.Serialization;

namespace TuneSpace.Core.DTOs.Responses.Spotify;

public record UserTopArtistsResponse(
    [property: JsonPropertyName("items")] List<SpotifyTopArtistResponse> Items
);
