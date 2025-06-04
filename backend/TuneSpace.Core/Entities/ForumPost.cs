namespace TuneSpace.Core.Entities;

public class ForumPost
{
    public Guid Id { get; set; }
    public Guid ThreadId { get; set; }
    public Guid AuthorId { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ForumThread Thread { get; set; } = null!;
    public User Author { get; set; } = null!;
    public ICollection<ForumPostLike> Likes { get; set; } = [];
}
