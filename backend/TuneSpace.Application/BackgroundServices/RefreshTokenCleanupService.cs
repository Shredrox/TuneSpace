using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TuneSpace.Core.Interfaces.IRepositories;

namespace TuneSpace.Application.BackgroundServices;

public class RefreshTokenCleanupService(
    ILogger<RefreshTokenCleanupService> logger,
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly ILogger<RefreshTokenCleanupService> _logger = logger;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RefreshTokenCleanupService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var _refreshTokenRepository = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();

                var deletedCount = await _refreshTokenRepository.DeleteExpiredAsync();

                _logger.LogInformation("Deleted {Count} expired refresh tokens.", deletedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting expired refresh tokens.");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }

        _logger.LogInformation("RefreshTokenCleanupService stopped.");
    }
}
