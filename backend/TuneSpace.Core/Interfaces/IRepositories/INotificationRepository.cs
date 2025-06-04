using TuneSpace.Core.Entities;

namespace TuneSpace.Core.Interfaces.IRepositories;

/// <summary>
/// Provides data access methods for managing notifications in the TuneSpace application.
/// </summary>
public interface INotificationRepository
{
    /// <summary>
    /// Retrieves a notification by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the notification to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the notification if found; otherwise, null.</returns>
    Task<Notification?> GetNotificationByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all notifications for a specific user.
    /// </summary>
    /// <param name="user">The user whose notifications should be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of notifications belonging to the user.</returns>
    Task<List<Notification>> GetNotificationsByUserAsync(User user);

    /// <summary>
    /// Creates a new notification in the system.
    /// </summary>
    /// <param name="notification">The notification entity to be inserted.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task InsertNotificationAsync(Notification notification);

    /// <summary>
    /// Updates a batch of notifications.
    /// </summary>
    /// <param name="notifications">The list of notifications to be updated.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateNotificationsAsync(List<Notification> notifications);

    /// <summary>
    /// Deletes a notification from the system by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the notification to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteNotificationAsync(Guid id);
}
