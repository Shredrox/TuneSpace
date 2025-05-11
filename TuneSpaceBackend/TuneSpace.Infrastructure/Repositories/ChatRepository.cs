using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

internal class ChatRepository(TuneSpaceDbContext context) : IChatRepository
{
    private readonly TuneSpaceDbContext _context = context;

    async Task IChatRepository.InsertChat(Chat chat)
    {
        _context.Chats.Add(chat);
        await _context.SaveChangesAsync();
    }

    async Task<Chat?> IChatRepository.GetChatById(Guid chatId)
    {
        return await _context.Chats
            .Include(c => c.User1)
            .Include(c => c.User2)
            .FirstOrDefaultAsync(c => c.Id == chatId);
    }

    async Task<List<Chat>> IChatRepository.GetChatsByUser1IdOrUser2Id(User user1, User user2)
    {
        return await _context.Chats
            .Include(c => c.User1)
            .Include(c => c.User2)
            .Where(c => c.User1 == user1 || c.User2 == user2)
            .ToListAsync();
    }

    async Task<Chat?> IChatRepository.GetChatByUser1AndUser2(User user1, User user2)
    {
        return await _context.Chats
            .FirstOrDefaultAsync(c => (c.User1 == user1 && c.User2 == user2) || c.User1 == user2 && c.User2 == user1);
    }
}
