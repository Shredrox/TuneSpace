using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TuneSpace.Infrastructure.Data;
using TuneSpace.Infrastructure.Options;

namespace TuneSpace.Infrastructure.Extensions;

public static class WebApplicationExtensions
{
    public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<WebApplication>>();
        var databaseOptions = services.GetRequiredService<IOptionsSnapshot<DatabaseOptions>>().Value;

        try
        {
            var context = services.GetRequiredService<TuneSpaceDbContext>();

            if (databaseOptions.AutoMigrate)
            {
                await ApplyMigrationsAsync(context, logger);
            }
            else
            {
                logger.LogInformation("Auto-migration is disabled in configuration");
            }


        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database initialization");

            if (!app.Environment.IsDevelopment())
            {
                throw;
            }

            logger.LogWarning("Database initialization failed in development environment, continuing startup...");
        }

        return app;
    }

    private static async Task ApplyMigrationsAsync(TuneSpaceDbContext context, ILogger logger)
    {
        logger.LogInformation("Checking for pending database migrations...");

        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            logger.LogInformation("Found {MigrationCount} pending migrations: {Migrations}",
                pendingMigrations.Count(),
                string.Join(", ", pendingMigrations)
            );

            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully");
        }
        else
        {
            logger.LogInformation("Database is up to date, no pending migrations found");
        }
    }
}
