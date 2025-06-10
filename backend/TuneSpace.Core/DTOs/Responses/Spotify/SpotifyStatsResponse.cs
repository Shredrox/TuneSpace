namespace TuneSpace.Core.DTOs.Responses.Spotify;

public record SpotifyStatsResponse(
    SpotifyProfileDTO Profile,
    List<SpotifyArtistDTO> TopArtists,
    List<TopSongDTO> TopSongs);
