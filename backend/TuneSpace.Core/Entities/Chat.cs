namespace TuneSpace.Core.Entities;

public class Chat
{
    public Guid Id { get; set; }
    public Guid ParticipantAId { get; set; }
    public Guid ParticipantBId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User ParticipantA { get; set; } = null!;
    public User ParticipantB { get; set; } = null!;
}
