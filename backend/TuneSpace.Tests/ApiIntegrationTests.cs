using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace TuneSpace.Tests;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Swagger_Endpoint_Is_Accessible_In_Development()
    {
        var response = await _client.GetAsync("/swagger/index.html");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void Service_Container_Can_Be_Built()
    {
        using var scope = _factory.Services.CreateScope();
        var services = scope.ServiceProvider;

        services.Should().NotBeNull();
    }

    [Fact]
    public async Task API_Returns_NotFound_For_Invalid_Endpoints()
    {
        var response = await _client.GetAsync("/api/nonexistent");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
