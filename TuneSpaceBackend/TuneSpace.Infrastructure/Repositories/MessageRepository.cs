using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

internal class MessageRepository(TuneSpaceDbContext context) : IMessageRepository
{
    private readonly TuneSpaceDbContext _context = context;

    async Task<Message> IMessageRepository.InsertMessage(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    async Task<List<Message>> IMessageRepository.GetMessagesBetweenUsersAsync(Guid userId, Guid otherUserId)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .Where(m =>
                (m.SenderId == userId && m.RecipientId == otherUserId) ||
                (m.SenderId == otherUserId && m.RecipientId == userId))
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
    }

    async Task<List<Message>> IMessageRepository.GetMessagesByChatIdAsync(Guid chatId)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
    }

    async Task<List<Guid>> IMessageRepository.GetConversationPartnerIdsAsync(Guid userId)
    {
        return await _context.Messages
            .Where(m => m.SenderId == userId || m.RecipientId == userId)
            .Select(m => m.SenderId == userId ? m.RecipientId : m.SenderId)
            .Distinct()
            .ToListAsync();
    }

    async Task<Message?> IMessageRepository.GetLatestMessageBetweenUsersAsync(Guid userId, Guid partnerId)
    {
        return await _context.Messages
            .Where(m =>
                (m.SenderId == userId && m.RecipientId == partnerId) ||
                (m.SenderId == partnerId && m.RecipientId == userId))
            .OrderByDescending(m => m.Timestamp)
            .FirstOrDefaultAsync();
    }

    async Task<int> IMessageRepository.GetUnreadMessageCountFromUserAsync(Guid senderId, Guid recipientId, Guid chatId)
    {
        return await _context.Messages
            .CountAsync(m => m.SenderId == senderId && m.RecipientId == recipientId && !m.IsRead && m.ChatId == chatId);
    }

    async Task<List<Message>> IMessageRepository.GetUnreadMessagesAsync(Guid senderId, Guid recipientId)
    {
        return await _context.Messages
            .Where(m => m.SenderId == senderId && m.RecipientId == recipientId && !m.IsRead)
            .ToListAsync();
    }

    async Task<int> IMessageRepository.GetTotalUnreadMessageCountAsync(Guid userId)
    {
        return await _context.Messages
            .CountAsync(m => m.RecipientId == userId && !m.IsRead);
    }

    async Task<bool> IMessageRepository.UpdateMessagesAsync(IEnumerable<Message> messages)
    {
        _context.Messages.UpdateRange(messages);
        var saved = await _context.SaveChangesAsync();
        return saved > 0;
    }

    async Task IMessageRepository.MarkMessagesAsReadAsync(Guid chatId, string senderId, string recipientId)
    {
        var messages = await _context.Messages
            .Where(m => m.ChatId == chatId &&
                       m.SenderId.ToString() == senderId &&
                       m.RecipientId.ToString() == recipientId &&
                       !m.IsRead)
            .ToListAsync();


        foreach (var message in messages)
        {
            message.IsRead = true;
        }

        _context.Messages.UpdateRange(messages);
        var saved = await _context.SaveChangesAsync();
    }
}
