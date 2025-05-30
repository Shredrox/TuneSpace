﻿namespace TuneSpace.Core.Entities;

public class Chat
{
    public Guid Id { get; set; }
    public Guid User1Id { get; set; }
    public User User1 { get; set; }
    public Guid User2Id { get; set; }
    public User User2 { get; set; }
    public DateTime CreatedAt { get; set; }
}
