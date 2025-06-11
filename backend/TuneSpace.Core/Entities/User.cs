using Microsoft.AspNetCore.Identity;
using TuneSpace.Core.Enums;

namespace TuneSpace.Core.Entities;

public class User : IdentityUser<Guid>
{
    public Roles Role { get; set; }
    public byte[]? ProfilePicture { get; set; }
    public string? SpotifyId { get; set; }
    public string? ExternalProvider { get; set; }
    public DateTime? ExternalLoginLinkedAt { get; set; }
    public DateTime LastActiveDate { get; set; } = DateTime.UtcNow;
}
