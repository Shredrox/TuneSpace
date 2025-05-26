using TuneSpace.Core.DTOs.Requests.Notification;
using TuneSpace.Core.DTOs.Responses.Notification;

namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Service interface for managing notifications in the system.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Retrieves all notifications for a specific user.
    /// </summary>
    /// <param name="username">The username of the user whose notifications should be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of notification response DTOs.</returns>
    Task<List<NotificationResponseDto>> GetUserNotificationsAsync(string username);

    /// <summary>
    /// Creates a new notification in the system.
    /// </summary>
    /// <param name="request">The request DTO containing the notification data to be created.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created notification as a response DTO.</returns>
    Task<NotificationResponseDto> CreateNotificationAsync(AddNotificationRequestDto request);

    /// <summary>
    /// Marks a specific notification as read.
    /// </summary>
    /// <param name="id">The unique identifier of the notification to mark as read.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ReadNotificationAsync(Guid id);

    /// <summary>
    /// Marks all notifications for a specific user as read.
    /// </summary>
    /// <param name="username">The username of the user whose notifications should be marked as read.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ReadNotificationsAsync(string username);

    /// <summary>
    /// Deletes a notification from the system by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the notification to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteNotificationAsync(Guid id);
}
