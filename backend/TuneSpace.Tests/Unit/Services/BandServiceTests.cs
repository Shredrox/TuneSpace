using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TuneSpace.Application.Services;
using TuneSpace.Core.DTOs.Responses.Band;
using TuneSpace.Core.DTOs.Responses.User;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Enums;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Tests.Unit.Services;

public class BandServiceTests
{
    private readonly Mock<IBandRepository> _mockBandRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ILogger<BandService>> _mockLogger;
    private readonly IBandService _bandService;

    public BandServiceTests()
    {
        _mockBandRepository = new Mock<IBandRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<BandService>>();
        _bandService = new BandService(_mockBandRepository.Object, _mockUserRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetBandByIdAsync_WithValidId_ShouldReturnBandResponse()
    {
        var bandId = Guid.NewGuid();
        var band = new Band
        {
            Id = bandId,
            Name = "Test Band",
            Description = "A test band",
            Genre = "Rock",
            Country = "USA",
            City = "New York",
            CoverImage = new byte[] { 1, 2, 3 },
            SpotifyId = "spotify123",
            YouTubeEmbedId = "youtube123",
            Members = new List<User>
            {
                new() { Id = Guid.NewGuid(), UserName = "member1", ProfilePicture = new byte[] { 4, 5, 6 } },
                new() { Id = Guid.NewGuid(), UserName = "member2", ProfilePicture = null }
            }
        };

        _mockBandRepository
            .Setup(x => x.GetBandByIdAsync(bandId))
            .ReturnsAsync(band);

        var result = await _bandService.GetBandByIdAsync(bandId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(bandId.ToString());
        result.Name.Should().Be("Test Band");
        result.Description.Should().Be("A test band");
        result.Genre.Should().Be("Rock");
        result.Country.Should().Be("USA");
        result.City.Should().Be("New York");
        result.CoverImage.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        result.SpotifyId.Should().Be("spotify123");
        result.YouTubeEmbedId.Should().Be("youtube123"); result.Members.Should().HaveCount(2);
        result.Members!.First().Name.Should().Be("member1");

        _mockBandRepository.Verify(x => x.GetBandByIdAsync(bandId), Times.Once);
    }

    [Fact]
    public async Task GetBandByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        var bandId = Guid.NewGuid();
        _mockBandRepository
            .Setup(x => x.GetBandByIdAsync(bandId))
            .ReturnsAsync((Band?)null);

        var result = await _bandService.GetBandByIdAsync(bandId);

        result.Should().BeNull();
        _mockBandRepository.Verify(x => x.GetBandByIdAsync(bandId), Times.Once);
    }

    [Fact]
    public async Task GetBandByNameAsync_WithValidName_ShouldReturnBandResponse()
    {
        var bandName = "Test Band";
        var band = new Band
        {
            Id = Guid.NewGuid(),
            Name = bandName,
            Description = "A test band",
            Genre = "Rock",
            Country = "USA",
            City = "New York"
        };

        _mockBandRepository
            .Setup(x => x.GetBandByNameAsync(bandName))
            .ReturnsAsync(band);

        var result = await _bandService.GetBandByNameAsync(bandName);

        result.Should().NotBeNull();
        result!.Name.Should().Be(bandName);
        result.Description.Should().Be("A test band");
        result.Genre.Should().Be("Rock");

        _mockBandRepository.Verify(x => x.GetBandByNameAsync(bandName), Times.Once);
    }

    [Fact]
    public async Task GetBandByNameAsync_WithInvalidName_ShouldReturnNull()
    {
        var bandName = "Non-existent Band";
        _mockBandRepository
            .Setup(x => x.GetBandByNameAsync(bandName))
            .ReturnsAsync((Band?)null);

        var result = await _bandService.GetBandByNameAsync(bandName);

        result.Should().BeNull();
        _mockBandRepository.Verify(x => x.GetBandByNameAsync(bandName), Times.Once);
    }

    [Fact]
    public async Task GetBandByUserIdAsync_WithValidUserId_ShouldReturnBandResponse()
    {
        var userId = Guid.NewGuid().ToString();
        var band = new Band
        {
            Id = Guid.NewGuid(),
            Name = "User's Band",
            Description = "Band owned by user",
            Genre = "Pop",
            Country = "Canada",
            City = "Toronto"
        };

        _mockBandRepository
            .Setup(x => x.GetBandByUserIdAsync(userId))
            .ReturnsAsync(band);

        var result = await _bandService.GetBandByUserIdAsync(userId);

        result.Should().NotBeNull();
        result!.Name.Should().Be("User's Band");
        result.Description.Should().Be("Band owned by user");
        result.Genre.Should().Be("Pop");

        _mockBandRepository.Verify(x => x.GetBandByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetBandByUserIdAsync_WithInvalidUserId_ShouldReturnNull()
    {
        var userId = Guid.NewGuid().ToString();
        _mockBandRepository
            .Setup(x => x.GetBandByUserIdAsync(userId))
            .ReturnsAsync((Band?)null);

        var result = await _bandService.GetBandByUserIdAsync(userId);

        result.Should().BeNull();
        _mockBandRepository.Verify(x => x.GetBandByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetBandImageAsync_WithValidBandId_ShouldReturnImageData()
    {
        var bandId = Guid.NewGuid();
        var imageData = new byte[] { 1, 2, 3, 4, 5 };
        var band = new Band
        {
            Id = bandId,
            Name = "Test Band",
            Genre = "Rock",
            CoverImage = imageData
        };

        _mockBandRepository
            .Setup(x => x.GetBandByIdAsync(bandId))
            .ReturnsAsync(band);

        var result = await _bandService.GetBandImageAsync(bandId);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(imageData);
        _mockBandRepository.Verify(x => x.GetBandByIdAsync(bandId), Times.Once);
    }

    [Fact]
    public async Task GetBandImageAsync_WithBandWithoutImage_ShouldReturnNull()
    {
        var bandId = Guid.NewGuid();
        var band = new Band
        {
            Id = bandId,
            Name = "Test Band",
            Genre = "Rock",
            CoverImage = null
        };

        _mockBandRepository
            .Setup(x => x.GetBandByIdAsync(bandId))
            .ReturnsAsync(band);

        var result = await _bandService.GetBandImageAsync(bandId);

        result.Should().BeNull();
        _mockBandRepository.Verify(x => x.GetBandByIdAsync(bandId), Times.Once);
    }

    [Fact]
    public async Task GetBandImageAsync_WithInvalidBandId_ShouldReturnNull()
    {
        var bandId = Guid.NewGuid();
        _mockBandRepository
            .Setup(x => x.GetBandByIdAsync(bandId))
            .ReturnsAsync((Band?)null);

        var result = await _bandService.GetBandImageAsync(bandId);

        result.Should().BeNull();
        _mockBandRepository.Verify(x => x.GetBandByIdAsync(bandId), Times.Once);
    }
    [Fact]
    public async Task GetBandMembersAsync_WithValidBandId_ShouldReturnMembersList()
    {
        var bandId = Guid.NewGuid();
        var band = new Band
        {
            Id = bandId,
            Name = "Test Band",
            Genre = "Rock",
            Members = new List<User>
            {
                new() { Id = Guid.NewGuid(), UserName = "member1", Email = "member1@test.com", Role = Roles.BandMember },
                new() { Id = Guid.NewGuid(), UserName = "member2", Email = "member2@test.com", Role = Roles.BandAdmin },
                new() { Id = Guid.NewGuid(), UserName = "member3", Email = "member3@test.com", Role = Roles.BandMember }
            }
        };

        _mockBandRepository
            .Setup(x => x.GetBandByIdAsync(bandId))
            .ReturnsAsync(band);

        var result = await _bandService.GetBandMembersAsync(bandId);

        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("member1");
        result[1].Name.Should().Be("member2");
        result[2].Name.Should().Be("member3");

        _mockBandRepository.Verify(x => x.GetBandByIdAsync(bandId), Times.Once);
    }

    [Fact]
    public async Task GetBandMembersAsync_WithBandWithoutMembers_ShouldReturnEmptyArray()
    {
        var bandId = Guid.NewGuid();
        var band = new Band
        {
            Id = bandId,
            Name = "Test Band",
            Genre = "Rock",
            Members = new List<User>()
        };

        _mockBandRepository
            .Setup(x => x.GetBandByIdAsync(bandId))
            .ReturnsAsync(band);

        var result = await _bandService.GetBandMembersAsync(bandId);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _mockBandRepository.Verify(x => x.GetBandByIdAsync(bandId), Times.Once);
    }
}
