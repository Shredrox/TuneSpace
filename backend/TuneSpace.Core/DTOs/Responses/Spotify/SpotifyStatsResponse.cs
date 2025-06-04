namespace TuneSpace.Core.DTOs.Responses.Spotify;

public record SpotifyStatsResponse(
    SpotifyProfileDTO Profile,
    List<TopArtistDTO> TopArtists,
    List<TopSongDTO> TopSongs);
