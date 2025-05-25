using Microsoft.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

internal class NotificationRepository(TuneSpaceDbContext context) : INotificationRepository
{
    private readonly TuneSpaceDbContext _context = context;

    async Task INotificationRepository.InsertNotification(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    async Task<Notification?> INotificationRepository.GetNotificationById(Guid id)
    {
        return await _context.Notifications.FindAsync(id);
    }

    async Task<List<Notification>> INotificationRepository.GetNotificationsByUser(User user)
    {
        return await _context.Notifications
            .Where(n => n.User == user)
            .ToListAsync();
    }

    async Task INotificationRepository.UpdateNotifications(List<Notification> notifications)
    {
        _context.Notifications.UpdateRange(notifications);
        await _context.SaveChangesAsync();
    }

    async Task INotificationRepository.DeleteNotification(Guid id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }
}
