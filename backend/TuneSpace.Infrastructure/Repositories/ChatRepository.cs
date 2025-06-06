using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

internal class ChatRepository(TuneSpaceDbContext context) : IChatRepository
{
    private readonly TuneSpaceDbContext _context = context;

    async Task<Chat?> IChatRepository.GetChatByIdAsync(Guid chatId)
    {
        return await _context.Chats
            .Include(c => c.ParticipantA)
            .Include(c => c.ParticipantB)
            .FirstOrDefaultAsync(c => c.Id == chatId);
    }

    async Task<List<Chat>> IChatRepository.GetChatsByUser1IdOrUser2IdAsync(User user1, User user2)
    {
        return await _context.Chats
            .Include(c => c.ParticipantA)
            .Include(c => c.ParticipantB)
            .Where(c => c.ParticipantA == user1 || c.ParticipantB == user2)
            .ToListAsync();
    }

    async Task<Chat?> IChatRepository.GetChatByUser1AndUser2Async(User user1, User user2)
    {
        return await _context.Chats
            .FirstOrDefaultAsync(c => (c.ParticipantA == user1 && c.ParticipantB == user2) || c.ParticipantA == user2 && c.ParticipantB == user1);
    }

    async Task IChatRepository.InsertChatAsync(Chat chat)
    {
        _context.Chats.Add(chat);
        await _context.SaveChangesAsync();
    }
}
