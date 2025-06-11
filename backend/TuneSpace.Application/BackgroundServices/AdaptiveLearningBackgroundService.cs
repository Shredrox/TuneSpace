using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Application.BackgroundServices;

/// <summary>
/// Background service that periodically adapts user recommendation weights and analyzes genre evolution
/// </summary>
public class AdaptiveLearningBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<AdaptiveLearningBackgroundService> logger) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<AdaptiveLearningBackgroundService> _logger = logger;
    private readonly TimeSpan _adaptationInterval = TimeSpan.FromHours(6);
    private readonly TimeSpan _genreAnalysisInterval = TimeSpan.FromDays(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Adaptive Learning Background Service started");

        var lastAdaptationRun = DateTime.UtcNow;
        var lastGenreAnalysisRun = DateTime.UtcNow;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.UtcNow;

                if (now - lastAdaptationRun >= _adaptationInterval)
                {
                    await RunPeriodicAdaptationAsync();
                    lastAdaptationRun = now;
                }

                if (now - lastGenreAnalysisRun >= _genreAnalysisInterval)
                {
                    await RunGenreEvolutionAnalysisAsync();
                    lastGenreAnalysisRun = now;
                }

                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Adaptive Learning Background Service");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        _logger.LogInformation("Adaptive Learning Background Service stopped");
    }

    private async Task RunPeriodicAdaptationAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var adaptiveLearningService = scope.ServiceProvider.GetRequiredService<IAdaptiveLearningService>();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>(); try
        {
            _logger.LogInformation("Starting periodic adaptation for active users");

            var activeUsers = await GetActiveUsers(userService);

            var adaptationTasks = activeUsers.Select(async userId =>
            {
                try
                {
                    await adaptiveLearningService.TriggerPeriodicAdaptationAsync(userId);
                    _logger.LogDebug("Completed adaptation for user");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to adapt recommendations for user");
                }
            });

            await Task.WhenAll(adaptationTasks);
            _logger.LogInformation("Completed periodic adaptation for {UserCount} active users", activeUsers.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during periodic adaptation");
        }
    }

    private async Task RunGenreEvolutionAnalysisAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var adaptiveLearningService = scope.ServiceProvider.GetRequiredService<IAdaptiveLearningService>();
        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

        try
        {
            _logger.LogInformation("Starting genre evolution analysis");

            var activeUsers = await GetActiveUsers(userService);

            foreach (var userId in activeUsers)
            {
                try
                {
                    var trends = await adaptiveLearningService.GetGenreTrendsAsync(userId, 90);

                    var emergingGenres = await adaptiveLearningService.GetEmergingGenresForUserAsync(userId);

                    var currentExploration = await adaptiveLearningService.GetExplorationFactorAsync(userId);

                    if (emergingGenres.Count != 0)
                    {
                        _logger.LogInformation("User has {Count} emerging genres", emergingGenres.Count);
                    }

                    if (trends.Any(t => Math.Abs(t.Value) > 0.05))
                    {
                        var significantTrends = trends.Where(t => Math.Abs(t.Value) > 0.05);
                        _logger.LogInformation("User has significant genre trend changes");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed genre evolution analysis for user");
                }
            }

            _logger.LogInformation("Completed genre evolution analysis for {UserCount} users", activeUsers.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during genre evolution analysis");
        }
    }

    private async Task<List<string>> GetActiveUsers(IUserService userService)
    {
        try
        {
            var activeUserIds = await userService.GetActiveUserIdsAsync(30);
            _logger.LogInformation("Retrieved {Count} active users for adaptive learning", activeUserIds.Count);
            return activeUserIds;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get active users, using empty list");
            return [];
        }
    }
}
