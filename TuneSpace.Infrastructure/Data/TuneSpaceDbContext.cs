using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;

namespace TuneSpace.Infrastructure.Data;

public class TuneSpaceDbContext(DbContextOptions<TuneSpaceDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Band> Bands { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<Follow> Follows { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }
}