using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

namespace TuneSpace.Tests.Integration;

public class UserControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public UserControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    [Fact]
    public async Task GetUserByName_WithoutAuthorization_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/User/testuser");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUsersBySearch_WithoutAuthorization_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/User/search/test");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProfilePicture_WithoutAuthorization_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/User/testuser/profile-picture");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUserProfile_WithoutAuthorization_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/User/testuser/profile");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUsersBySearch_WithEmptySearch_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/User/search/");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
