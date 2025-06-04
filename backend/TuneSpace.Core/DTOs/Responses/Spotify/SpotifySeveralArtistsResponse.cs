using System.Text.Json.Serialization;

namespace TuneSpace.Core.DTOs.Responses.Spotify;

public record SpotifySeveralArtistsResponse(
    [property: JsonPropertyName("artists")] List<SpotifyArtistDTO> Artists
);
