using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Infrastructure.Identity;

namespace TuneSpace.Infrastructure.Data;

public class TuneSpaceDbContext(DbContextOptions<TuneSpaceDbContext> options) : IdentityDbContext<User, ApplicationRole, Guid>(options)
{
    public DbSet<Band> Bands { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Follow> Follows { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<MusicEvent> MusicEvents { get; set; }
}
