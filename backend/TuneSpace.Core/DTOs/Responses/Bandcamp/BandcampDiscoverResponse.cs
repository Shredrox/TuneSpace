using System.Text.Json.Serialization;

namespace TuneSpace.Core.DTOs.Responses.Bandcamp;

public class BandcampDiscoverResponse
{
    [JsonPropertyName("results")]
    public List<BandcampDiscoverItem> Results { get; set; } = [];

    [JsonPropertyName("more_available")]
    public bool MoreAvailable { get; set; }

    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }
}

public class BandcampDiscoverItem
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("is_album_preorder")]
    public bool IsAlbumPreorder { get; set; }

    [JsonPropertyName("is_free_download")]
    public bool IsFreeDownload { get; set; }

    [JsonPropertyName("is_purchasable")]
    public bool IsPurchasable { get; set; }

    [JsonPropertyName("is_set_price")]
    public bool IsSetPrice { get; set; }

    [JsonPropertyName("item_url")]
    public string ItemUrl { get; set; } = string.Empty;

    [JsonPropertyName("item_price")]
    public double? ItemPrice { get; set; }

    [JsonPropertyName("price")]
    public BandcampPrice? Price { get; set; }

    [JsonPropertyName("item_currency")]
    public string? ItemCurrency { get; set; }

    [JsonPropertyName("item_image_id")]
    public long? ItemImageId { get; set; }

    [JsonPropertyName("result_type")]
    public string ResultType { get; set; } = string.Empty;

    [JsonPropertyName("band_id")]
    public long BandId { get; set; }

    [JsonPropertyName("album_artist")]
    public string? AlbumArtist { get; set; }

    [JsonPropertyName("band_name")]
    public string BandName { get; set; } = string.Empty;

    [JsonPropertyName("band_url")]
    public string BandUrl { get; set; } = string.Empty;

    [JsonPropertyName("band_bio_image_id")]
    public long? BandBioImageId { get; set; }

    [JsonPropertyName("band_latest_art_id")]
    public long? BandLatestArtId { get; set; }

    [JsonPropertyName("band_genre_id")]
    public int? BandGenreId { get; set; }

    [JsonPropertyName("release_date")]
    public string? ReleaseDate { get; set; }

    [JsonPropertyName("total_package_count")]
    public int TotalPackageCount { get; set; }

    [JsonPropertyName("featured_track")]
    public BandcampTrack? FeaturedTrack { get; set; }

    [JsonPropertyName("band_location")]
    public string? BandLocation { get; set; }

    [JsonPropertyName("track_count")]
    public int? TrackCount { get; set; }

    [JsonPropertyName("item_duration")]
    public double? ItemDuration { get; set; }

    [JsonPropertyName("item_tags")]
    public List<BandcampTag>? ItemTags { get; set; }
}

public class BandcampPrice
{
    [JsonPropertyName("amount")]
    public int Amount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("is_money")]
    public bool IsMoney { get; set; }
}

public class BandcampUrlHints
{
    [JsonPropertyName("subdomain")]
    public string? Subdomain { get; set; }

    [JsonPropertyName("custom_domain")]
    public string? CustomDomain { get; set; }

    [JsonPropertyName("custom_domain_verified")]
    public bool? CustomDomainVerified { get; set; }
}

public class BandcampTrack
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("band_name")]
    public string BandName { get; set; } = string.Empty;

    [JsonPropertyName("band_id")]
    public long BandId { get; set; }

    [JsonPropertyName("duration")]
    public double Duration { get; set; }

    [JsonPropertyName("stream_url")]
    public string? StreamUrl { get; set; }
}

public class BandcampTag
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("norm_name")]
    public string NormName { get; set; } = string.Empty;
}
