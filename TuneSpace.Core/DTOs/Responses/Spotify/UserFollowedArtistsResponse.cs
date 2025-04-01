using Newtonsoft.Json;

namespace TuneSpace.Core.DTOs.Responses.Spotify;

public class UserFollowedArtistsResponse
{
    [JsonProperty("artists")]
    public ArtistsContainer Artists { get; set; }
}

public class ArtistsContainer
{
    [JsonProperty("href")]
    public string Href { get; set; }
    
    [JsonProperty("limit")]
    public int Limit { get; set; }
    
    [JsonProperty("next")]
    public string Next { get; set; }
    
    [JsonProperty("cursors")]
    public Cursors Cursors { get; set; }
    
    [JsonProperty("total")]
    public int Total { get; set; }
    
    [JsonProperty("items")]
    public List<ArtistItem> Items { get; set; }
}

public class Cursors
{
    [JsonProperty("after")]
    public string After { get; set; }
}

public class ArtistItem
{
    [JsonProperty("id")]
    public string Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("popularity")]
    public int Popularity { get; set; }
    
    [JsonProperty("images")]
    public List<SpotifyImage> Images { get; set; }
    
    [JsonProperty("genres")]
    public List<string> Genres { get; set; }
    
    [JsonProperty("followers")]
    public Followers Followers { get; set; }
}

public class Followers
{
    [JsonProperty("total")]
    public int Total { get; set; }
}
