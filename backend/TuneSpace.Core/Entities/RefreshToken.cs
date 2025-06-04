namespace TuneSpace.Core.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = null!;
    public DateTime Expiry { get; set; }
    public DateTime? RevokedAt { get; set; }

    public User User { get; set; } = null!;
}
