namespace TuneSpace.Core.Entities;

public class Post
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public ICollection<Tag> Tags { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }

    public Post()
    {
        Tags = new List<Tag>();
    }
}
