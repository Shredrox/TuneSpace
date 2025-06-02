namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class UserRecentlyPlayedTracksResponse
{
    public List<RecentlyPlayedItem> Items { get; set; } = [];
    public string Href { get; set; } = string.Empty;
    public int Limit { get; set; }
    public string Next { get; set; } = string.Empty;
    public RecentlyPlayedCursors Cursors { get; set; } = new();

    public class RecentlyPlayedItem
    {
        public TrackInfo Track { get; set; } = new();
        public string Played_At { get; set; } = string.Empty;

        public class TrackInfo
        {
            public string Name { get; set; } = string.Empty;
            public int Duration_Ms { get; set; }
            public List<ArtistInfo> Artists { get; set; } = [];
            public AlbumInfo Album { get; set; } = new();

            public class ArtistInfo
            {
                public string Name { get; set; } = string.Empty;
                public string Id { get; set; } = string.Empty;
            }

            public class AlbumInfo
            {
                public string Name { get; set; } = string.Empty;
                public List<ImageInfo> Images { get; set; } = [];

                public class ImageInfo
                {
                    public string Url { get; set; } = string.Empty;
                    public int Height { get; set; }
                    public int Width { get; set; }
                }
            }
        }
    }

    public class RecentlyPlayedCursors
    {
        public string After { get; set; } = string.Empty;
        public string Before { get; set; } = string.Empty;
    }
}
