using System.Text.Json.Serialization;

namespace TuneSpace.Core.DTOs.Responses.User;

public record UserProfileResponse(
    string Username,
    int FollowerCount,
    int FollowingCount,
    int PostsCount,
    int PlaylistsCount,
    string? FavoriteSong,
    string? FavoriteBand,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? ProfilePicture
);
