using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TuneSpace.Infrastructure.Clients;
using TuneSpace.Infrastructure.Data;
using TuneSpace.Infrastructure.Repositories;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IClients;
using TuneSpace.Core.Interfaces.IRepositories;

namespace TuneSpace.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TuneSpaceDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("TuneSpaceDb")));

        return services;
    }

    public static IServiceCollection AddCachingServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        return services;
    }

    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme);
        services.AddAuthorizationBuilder();

        services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<TuneSpaceDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBandRepository, BandRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ITagRepository, TagRepository>();

        return services;
    }

    public static IServiceCollection AddHttpClientServices(this IServiceCollection services)
    {
        services.AddHttpClient<IOllamaClient, OllamaClient>();
        services.AddHttpClient<ILastFmClient, LastFmClient>();
        services.AddHttpClient<IMusicBrainzClient, MusicBrainzClient>();
        services.AddHttpClient<ISpotifyClient, SpotifyClient>();

        return services;
    }
}
