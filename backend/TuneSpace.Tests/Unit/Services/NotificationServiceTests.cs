using FluentAssertions;
using Moq;
using TuneSpace.Application.Services;
using TuneSpace.Core.DTOs.Requests.Notification;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Exceptions;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Tests.Unit.Services;

public class NotificationServiceTests
{
    private readonly Mock<INotificationRepository> _mockNotificationRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly INotificationService _notificationService;

    public NotificationServiceTests()
    {
        _mockNotificationRepository = new Mock<INotificationRepository>();
        _mockUserRepository = new Mock<IUserRepository>();

        _notificationService = new NotificationService(
            _mockNotificationRepository.Object,
            _mockUserRepository.Object);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_WithValidUsername_ShouldReturnNotifications()
    {
        var username = "testuser";
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            UserName = username,
            Email = "test@example.com"
        };

        var notifications = new List<Notification>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Message = "Test notification 1",
                IsRead = false,
                Type = "info",
                Timestamp = DateTime.UtcNow.AddHours(-1),
                User = user,
                RecipientName = username
            },
            new()
            {
                Id = Guid.NewGuid(),
                Message = "Test notification 2",
                IsRead = true,
                Type = "warning",
                Timestamp = DateTime.UtcNow.AddHours(-2),
                User = user,
                RecipientName = username
            }
        };

        _mockUserRepository
            .Setup(x => x.GetUserByNameAsync(username))
            .ReturnsAsync(user);

        _mockNotificationRepository
            .Setup(x => x.GetNotificationsByUserAsync(user))
            .ReturnsAsync(notifications);

        var result = await _notificationService.GetUserNotificationsAsync(username);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Message.Should().Be("Test notification 1");
        result[0].IsRead.Should().BeFalse();
        result[0].Type.Should().Be("info");
        result[1].Message.Should().Be("Test notification 2");
        result[1].IsRead.Should().BeTrue();
        result[1].Type.Should().Be("warning");

        _mockUserRepository.Verify(x => x.GetUserByNameAsync(username), Times.Once);
        _mockNotificationRepository.Verify(x => x.GetNotificationsByUserAsync(user), Times.Once);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_WithInvalidUsername_ShouldThrowNotFoundException()
    {
        var username = "nonexistentuser";

        _mockUserRepository
            .Setup(x => x.GetUserByNameAsync(username))
            .ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _notificationService.GetUserNotificationsAsync(username));

        exception.Message.Should().Contain("User is not found");
        _mockUserRepository.Verify(x => x.GetUserByNameAsync(username), Times.Once);
    }

    [Fact]
    public async Task CreateNotificationAsync_WithValidRequest_ShouldCreateAndReturnNotification()
    {
        var request = new AddNotificationRequestDto(
            "New notification message",
            "info",
            "test",
            "testuser"
        );

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.RecipientName,
            Email = "test@example.com"
        };

        _mockUserRepository
            .Setup(x => x.GetUserByNameAsync(request.RecipientName))
            .ReturnsAsync(user);

        _mockNotificationRepository
            .Setup(x => x.InsertNotificationAsync(It.IsAny<Notification>()))
            .Returns(Task.CompletedTask);

        var result = await _notificationService.CreateNotificationAsync(request);

        result.Should().NotBeNull();
        result.Message.Should().Be(request.Message);
        result.Type.Should().Be(request.Type);
        result.IsRead.Should().BeFalse();

        _mockUserRepository.Verify(x => x.GetUserByNameAsync(request.RecipientName), Times.Once);
        _mockNotificationRepository.Verify(x => x.InsertNotificationAsync(It.Is<Notification>(n =>
            n.Message == request.Message &&
            n.Type == request.Type &&
            n.Source == request.Source &&
            n.RecipientName == request.RecipientName &&
            n.User == user &&
            n.IsRead == false
        )), Times.Once);
    }
    [Fact]
    public async Task CreateNotificationAsync_WithInvalidRecipient_ShouldThrowNotFoundException()
    {
        var request = new AddNotificationRequestDto(
            "New notification message",
            "info",
            "test",
            "nonexistentuser"
        );

        _mockUserRepository
            .Setup(x => x.GetUserByNameAsync(request.RecipientName))
            .ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _notificationService.CreateNotificationAsync(request));

        exception.Message.Should().Contain("User is not found");
        _mockUserRepository.Verify(x => x.GetUserByNameAsync(request.RecipientName), Times.Once);
        _mockNotificationRepository.Verify(x => x.InsertNotificationAsync(It.IsAny<Notification>()), Times.Never);
    }

    [Fact]
    public async Task ReadNotificationAsync_WithValidId_ShouldMarkNotificationAsRead()
    {
        var notificationId = Guid.NewGuid(); var notification = new Notification
        {
            Id = notificationId,
            Message = "Test notification",
            IsRead = false,
            Type = "info",
            Timestamp = DateTime.UtcNow,
            RecipientName = "testuser"
        };

        _mockNotificationRepository
            .Setup(x => x.GetNotificationByIdAsync(notificationId))
            .ReturnsAsync(notification);

        _mockNotificationRepository
            .Setup(x => x.UpdateNotificationsAsync(It.IsAny<List<Notification>>()))
            .Returns(Task.CompletedTask);

        await _notificationService.ReadNotificationAsync(notificationId);

        notification.IsRead.Should().BeTrue();

        _mockNotificationRepository.Verify(x => x.GetNotificationByIdAsync(notificationId), Times.Once);
        _mockNotificationRepository.Verify(x => x.UpdateNotificationsAsync(It.Is<List<Notification>>(list =>
            list.Count == 1 && list[0].Id == notificationId && list[0].IsRead == true
        )), Times.Once);
    }

    [Fact]
    public async Task ReadNotificationAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        var notificationId = Guid.NewGuid();

        _mockNotificationRepository
            .Setup(x => x.GetNotificationByIdAsync(notificationId))
            .ReturnsAsync((Notification?)null);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _notificationService.ReadNotificationAsync(notificationId));

        exception.Message.Should().Contain("Notification is not found");
        _mockNotificationRepository.Verify(x => x.GetNotificationByIdAsync(notificationId), Times.Once);
        _mockNotificationRepository.Verify(x => x.UpdateNotificationsAsync(It.IsAny<List<Notification>>()), Times.Never);
    }

    [Fact]
    public async Task ReadNotificationsAsync_WithValidUsername_ShouldMarkAllNotificationsAsRead()
    {
        var username = "testuser";
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = username,
            Email = "test@example.com"
        };

        var notifications = new List<Notification>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Message = "Test notification 1",
                IsRead = false,
                Type = "info",
                Timestamp = DateTime.UtcNow,
                RecipientName = username
            },
            new()
            {
                Id = Guid.NewGuid(),
                Message = "Test notification 2",
                IsRead = false,
                Type = "warning",
                Timestamp = DateTime.UtcNow,
                RecipientName = username
            }
        };

        _mockUserRepository
            .Setup(x => x.GetUserByNameAsync(username))
            .ReturnsAsync(user);

        _mockNotificationRepository
            .Setup(x => x.GetNotificationsByUserAsync(user))
            .ReturnsAsync(notifications);

        _mockNotificationRepository
            .Setup(x => x.UpdateNotificationsAsync(It.IsAny<List<Notification>>()))
            .Returns(Task.CompletedTask);

        await _notificationService.ReadNotificationsAsync(username);

        notifications.Should().AllSatisfy(n => n.IsRead.Should().BeTrue());

        _mockUserRepository.Verify(x => x.GetUserByNameAsync(username), Times.Once);
        _mockNotificationRepository.Verify(x => x.GetNotificationsByUserAsync(user), Times.Once);
        _mockNotificationRepository.Verify(x => x.UpdateNotificationsAsync(It.Is<List<Notification>>(list =>
            list.Count == 2 && list.All(n => n.IsRead)
        )), Times.Once);
    }

    [Fact]
    public async Task ReadNotificationsAsync_WithInvalidUsername_ShouldThrowNotFoundException()
    {
        var username = "nonexistentuser";

        _mockUserRepository
            .Setup(x => x.GetUserByNameAsync(username))
            .ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _notificationService.ReadNotificationsAsync(username));

        exception.Message.Should().Contain("User is not found");
        _mockUserRepository.Verify(x => x.GetUserByNameAsync(username), Times.Once);
        _mockNotificationRepository.Verify(x => x.GetNotificationsByUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task DeleteNotificationAsync_WithValidId_ShouldDeleteNotification()
    {
        var notificationId = Guid.NewGuid();

        _mockNotificationRepository
            .Setup(x => x.DeleteNotificationAsync(notificationId))
            .Returns(Task.CompletedTask);

        await _notificationService.DeleteNotificationAsync(notificationId);

        _mockNotificationRepository.Verify(x => x.DeleteNotificationAsync(notificationId), Times.Once);
    }
}
