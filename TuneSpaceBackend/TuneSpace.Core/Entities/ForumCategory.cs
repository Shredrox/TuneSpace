namespace TuneSpace.Core.Entities;

public class ForumCategory
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? IconName { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPinned { get; set; } = false;

    public ICollection<ForumThread> Threads { get; set; } = [];
}
