namespace TuneSpace.Core.Entities;

public class Tag
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Post> Posts { get; set; }
}
