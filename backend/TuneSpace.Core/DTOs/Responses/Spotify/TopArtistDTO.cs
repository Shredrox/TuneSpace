namespace TuneSpace.Core.DTOs.Responses.Spotify;

public record TopArtistDTO(
    string Name,
    int Popularity,
    string Image);
