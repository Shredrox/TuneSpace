using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TuneSpace.Application.Services;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Enums;
using TuneSpace.Core.Exceptions;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Tests.Unit.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly IUserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserService>>();
        _userService = new UserService(_mockUserRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithValidId_ShouldReturnUser()
    {
        var userId = Guid.NewGuid().ToString();
        var expectedUser = new User
        {
            Id = Guid.Parse(userId),
            UserName = "testuser",
            Email = "test@example.com",
            Role = Roles.Listener
        };

        _mockUserRepository
            .Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync(expectedUser);

        var result = await _userService.GetUserByIdAsync(userId);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedUser);
        _mockUserRepository.Verify(x => x.GetUserByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        var userId = Guid.NewGuid().ToString();
        _mockUserRepository
            .Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync((User?)null);

        var result = await _userService.GetUserByIdAsync(userId);

        result.Should().BeNull();
        _mockUserRepository.Verify(x => x.GetUserByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserByNameAsync_WithValidName_ShouldReturnUser()
    {
        var userName = "testuser";
        var expectedUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            Email = "test@example.com",
            Role = Roles.Listener
        };

        _mockUserRepository
            .Setup(x => x.GetUserByNameAsync(userName))
            .ReturnsAsync(expectedUser);

        var result = await _userService.GetUserByNameAsync(userName);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedUser);
        _mockUserRepository.Verify(x => x.GetUserByNameAsync(userName), Times.Once);
    }

    [Fact]
    public async Task GetUserByNameAsync_WithInvalidName_ShouldReturnNull()
    {
        var userName = "nonexistentuser";
        _mockUserRepository
            .Setup(x => x.GetUserByNameAsync(userName))
            .ReturnsAsync((User?)null);

        var result = await _userService.GetUserByNameAsync(userName);

        result.Should().BeNull();
        _mockUserRepository.Verify(x => x.GetUserByNameAsync(userName), Times.Once);
    }

    [Fact]
    public async Task GetProfilePictureAsync_WithValidUser_ShouldReturnProfilePicture()
    {
        var userName = "testuser";
        var profilePicture = new byte[] { 1, 2, 3, 4, 5 };
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = userName,
            Email = "test@example.com",
            Role = Roles.Listener,
            ProfilePicture = profilePicture
        };

        _mockUserRepository
            .Setup(x => x.GetUserByNameAsync(userName))
            .ReturnsAsync(user);

        var result = await _userService.GetProfilePictureAsync(userName);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(profilePicture);
        _mockUserRepository.Verify(x => x.GetUserByNameAsync(userName), Times.Once);
    }

    [Fact]
    public async Task GetProfilePictureAsync_WithNonExistentUser_ShouldThrowNotFoundException()
    {
        var userName = "nonexistentuser";
        _mockUserRepository
            .Setup(x => x.GetUserByNameAsync(userName))
            .ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _userService.GetProfilePictureAsync(userName));

        exception.Message.Should().Contain($"User not found: {userName}");
        _mockUserRepository.Verify(x => x.GetUserByNameAsync(userName), Times.Once);
    }

    [Fact]
    public async Task SearchByNameAsync_WithMatchingUsers_ShouldReturnFilteredUsers()
    {
        var searchName = "test";
        var currentUserId = Guid.NewGuid().ToString();
        var users = new List<User>
        {
            new() { Id = Guid.NewGuid(), UserName = "testuser1", Email = "test1@example.com", Role = Roles.Listener },
            new() { Id = Guid.Parse(currentUserId), UserName = "testuser2", Email = "test2@example.com", Role = Roles.Listener },
            new() { Id = Guid.NewGuid(), UserName = "testuser3", Email = "test3@example.com", Role = Roles.Listener }
        };

        _mockUserRepository
            .Setup(x => x.SearchByNameAsync(searchName))
            .ReturnsAsync(users);

        var result = await _userService.SearchByNameAsync(searchName, currentUserId);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().NotContain(u => u.Id.ToString() == currentUserId);
        _mockUserRepository.Verify(x => x.SearchByNameAsync(searchName), Times.Once);
    }

    [Fact]
    public async Task SearchByNameAsync_WithNoMatches_ShouldReturnEmptyList()
    {
        var searchName = "nonexistent";
        var currentUserId = Guid.NewGuid().ToString();

        _mockUserRepository
            .Setup(x => x.SearchByNameAsync(searchName))
            .ReturnsAsync(new List<User>());

        var result = await _userService.SearchByNameAsync(searchName, currentUserId);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _mockUserRepository.Verify(x => x.SearchByNameAsync(searchName), Times.Once);
    }

    [Fact]
    public async Task SearchByNameAsync_WithNullCurrentUserId_ShouldReturnAllUsers()
    {
        var searchName = "test";
        var users = new List<User>
        {
            new() { Id = Guid.NewGuid(), UserName = "testuser1", Email = "test1@example.com", Role = Roles.Listener },
            new() { Id = Guid.NewGuid(), UserName = "testuser2", Email = "test2@example.com", Role = Roles.Listener }
        };

        _mockUserRepository
            .Setup(x => x.SearchByNameAsync(searchName))
            .ReturnsAsync(users);

        var result = await _userService.SearchByNameAsync(searchName, null);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(users);
        _mockUserRepository.Verify(x => x.SearchByNameAsync(searchName), Times.Once);
    }
}
