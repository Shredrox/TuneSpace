using System.Text.Json.Serialization;

namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class UserRecentlyPlayedTracksResponse
{
    [JsonPropertyName("items")]
    public List<RecentlyPlayedItem> Items { get; set; } = [];

    [JsonPropertyName("href")]
    public string Href { get; set; } = string.Empty;

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("next")]
    public string Next { get; set; } = string.Empty;

    [JsonPropertyName("cursors")]
    public RecentlyPlayedCursors Cursors { get; set; } = new();

    public class RecentlyPlayedItem
    {
        [JsonPropertyName("track")]
        public TrackInfo Track { get; set; } = new();

        [JsonPropertyName("played_at")]
        public string Played_At { get; set; } = string.Empty;

        public class TrackInfo
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("duration_ms")]
            public int Duration_Ms { get; set; }

            [JsonPropertyName("artists")]
            public List<ArtistInfo> Artists { get; set; } = [];

            [JsonPropertyName("album")]
            public AlbumInfo Album { get; set; } = new();

            public class ArtistInfo
            {
                [JsonPropertyName("name")]
                public string Name { get; set; } = string.Empty;

                [JsonPropertyName("id")]
                public string Id { get; set; } = string.Empty;
            }

            public class AlbumInfo
            {
                [JsonPropertyName("name")]
                public string Name { get; set; } = string.Empty;

                [JsonPropertyName("images")]
                public List<ImageInfo> Images { get; set; } = [];

                public class ImageInfo
                {
                    [JsonPropertyName("url")]
                    public string Url { get; set; } = string.Empty;

                    [JsonPropertyName("height")]
                    public int Height { get; set; }

                    [JsonPropertyName("width")]
                    public int Width { get; set; }
                }
            }
        }
    }

    public class RecentlyPlayedCursors
    {
        [JsonPropertyName("after")]
        public string After { get; set; } = string.Empty;

        [JsonPropertyName("before")]
        public string Before { get; set; } = string.Empty;
    }
}
