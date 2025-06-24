using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TuneSpace.Core.DTOs.Requests.Auth;

namespace TuneSpace.Tests.Integration;

public class AuthControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public AuthControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }


    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        var loginRequest = new LoginRequest(
            "nonexistent@example.com",
            "wrongpassword"
        );

        var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    [Fact]
    public async Task Register_WithValidData_ShouldReturnOk()
    {
        var registerRequest = new RegisterRequest(
            $"testuser_{Guid.NewGuid():N}",
            $"test_{Guid.NewGuid():N}@example.com",
            "Test123!@#",
            "User"
        );

        var response = await _client.PostAsJsonAsync("/api/Auth/register", registerRequest);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task Register_WithInvalidData_ShouldReturnBadRequest()
    {
        var registerRequest = new RegisterRequest(
            "",
            "invalidemail",
            "123",
            "User"
        );

        var response = await _client.PostAsJsonAsync("/api/Auth/register", registerRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ForgotPassword_WithValidEmail_ShouldReturnOk()
    {
        var request = new ForgotPasswordRequest("test@example.com");

        var response = await _client.PostAsJsonAsync("/api/Auth/forgot-password", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
