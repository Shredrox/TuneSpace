namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class SpotifySongSearchResponse
{
    public SearchItems Tracks { get; set; }
}

public class SearchItems
{
    public List<SearchSong> Items { get; set; }
}

public class SearchSong
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<Artist> Artists { get; set; }
    public SearchSongAlbum Album { get; set; }
}

public class SongSearchImageDTO
{
    public int Height { get; set; }
    public string? Url { get; set; }
    public int Width { get; set; }
}

public class SearchSongAlbum
{
    public List<SongSearchImageDTO> Images { get; set; }
}
