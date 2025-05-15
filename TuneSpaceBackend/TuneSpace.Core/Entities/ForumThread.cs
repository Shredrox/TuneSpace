namespace TuneSpace.Core.Entities;

public class ForumThread
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public int Views { get; set; }
    public bool IsPinned { get; set; }
    public bool IsLocked { get; set; }
    public Guid CategoryId { get; set; }
    public Guid AuthorId { get; set; }
    public ForumCategory Category { get; set; } = null!;
    public User Author { get; set; } = null!;
    public ICollection<ForumPost> Posts { get; set; } = [];
}
