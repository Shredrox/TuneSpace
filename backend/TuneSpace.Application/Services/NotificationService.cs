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

    async Task<List<NotificationResponseDto>> INotificationService.GetUserNotificationsAsync(string username)
    {
        var user = await _userRepository.GetUserByNameAsync(username) ?? throw new NotFoundException("User is not found");
        var notifications = await _notificationRepository.GetNotificationsByUserAsync(user);

        var response = notifications
            .Select(n => new NotificationResponseDto(n.Id, n.Message ?? "", n.IsRead, n.Type ?? "", n.Timestamp))
            .ToList();

        return response;
    }

    async Task<NotificationResponseDto> INotificationService.CreateNotificationAsync(AddNotificationRequestDto request)
    {
        var user = await _userRepository.GetUserByNameAsync(request.RecipientName) ?? throw new NotFoundException("User is not found");
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

        await _notificationRepository.InsertNotificationAsync(notification);

        return new NotificationResponseDto(
            notification.Id,
            notification.Message,
            notification.IsRead,
            notification.Type,
            notification.Timestamp
        );
    }

    async Task INotificationService.ReadNotificationAsync(Guid id)
    {
        var notification = await _notificationRepository.GetNotificationByIdAsync(id) ?? throw new NotFoundException("Notification is not found");

        notification.IsRead = true;

        await _notificationRepository.UpdateNotificationsAsync([notification]);
    }

    async Task INotificationService.ReadNotificationsAsync(string username)
    {
        var user = await _userRepository.GetUserByNameAsync(username) ?? throw new NotFoundException("User is not found");
        var notifications = await _notificationRepository.GetNotificationsByUserAsync(user);

        notifications.ForEach(n => n.IsRead = true);

        await _notificationRepository.UpdateNotificationsAsync(notifications);
    }

    async Task INotificationService.DeleteNotificationAsync(Guid id)
    {
        await _notificationRepository.DeleteNotificationAsync(id);
    }
}
