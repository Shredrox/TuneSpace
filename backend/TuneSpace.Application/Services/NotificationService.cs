using TuneSpace.Core.DTOs.Requests.Notification;
using TuneSpace.Core.DTOs.Responses.Notification;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Exceptions;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Application.Services;

internal class NotificationService(
    INotificationRepository notificationRepository,
    IUserRepository userRepository) : INotificationService
{
    private readonly INotificationRepository _notificationRepository = notificationRepository;
    private readonly IUserRepository _userRepository = userRepository;

    async Task<NotificationResponseDto> INotificationService.CreateNotification(AddNotificationRequestDto request)
    {
        var user = await _userRepository.GetUserByName(request.RecipientName) ?? throw new NotFoundException("User is not found");
        var notification = new Notification
        {
            Message = request.Message,
            Type = request.Type,
            Source = request.Source,
            RecipientName = request.RecipientName,
            User = user,
            IsRead = false,
            Timestamp = DateTime.Now.ToUniversalTime()
        };

        await _notificationRepository.InsertNotification(notification);

        return new NotificationResponseDto(
            notification.Id,
            notification.Message,
            notification.IsRead,
            notification.Type,
            notification.Timestamp
        );
    }

    async Task<List<NotificationResponseDto>> INotificationService.GetUserNotifications(string username)
    {
        var user = await _userRepository.GetUserByName(username) ?? throw new NotFoundException("User is not found");
        var notifications = await _notificationRepository.GetNotificationsByUser(user);

        var response = notifications
            .Select(n => new NotificationResponseDto(n.Id, n.Message, n.IsRead, n.Type, n.Timestamp))
            .ToList();

        return response;
    }

    async Task INotificationService.ReadNotification(Guid id)
    {
        var notification = await _notificationRepository.GetNotificationById(id) ?? throw new NotFoundException("Notification is not found");

        notification.IsRead = true;

        await _notificationRepository.UpdateNotifications([notification]);
    }

    async Task INotificationService.ReadNotifications(string username)
    {
        var user = await _userRepository.GetUserByName(username) ?? throw new NotFoundException("User is not found");
        var notifications = await _notificationRepository.GetNotificationsByUser(user);

        notifications.ForEach(n => n.IsRead = true);

        await _notificationRepository.UpdateNotifications(notifications);
    }

    async Task INotificationService.DeleteNotification(Guid id)
    {
        await _notificationRepository.DeleteNotification(id);
    }
}
