namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class UserRecentlyPlayedTracksResponse
{
    public List<RecentlyPlayedItem> Items { get; set; }

    public class RecentlyPlayedItem
    {
        public TrackInfo Track { get; set; }
        public string Played_At { get; set; }

        public class TrackInfo
        {
            public string Name { get; set; }
            public List<ArtistInfo> Artists { get; set; }
            public AlbumInfo Album { get; set; }

            public class ArtistInfo
            {
                public string Name { get; set; }
                public string Id { get; set; }
            }

            public class AlbumInfo
            {
                public string Name { get; set; }
                public List<ImageInfo> Images { get; set; }

                public class ImageInfo
                {
                    public string Url { get; set; }
                    public int Height { get; set; }
                    public int Width { get; set; }
                }
            }
        }
    }
}
