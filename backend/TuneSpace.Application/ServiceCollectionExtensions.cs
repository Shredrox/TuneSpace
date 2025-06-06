using Microsoft.Extensions.DependencyInjection;
using TuneSpace.Core.Interfaces.IServices;
using TuneSpace.Core.Interfaces.IInfrastructure;
using TuneSpace.Application.Services;
using TuneSpace.Application.Services.MusicDiscovery;
using TuneSpace.Application.BackgroundServices;

namespace TuneSpace.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IApiThrottler, ApiThrottler>();
        services.AddSingleton<IBandCachingService, BandCachingService>();

        services.AddScoped<IDataEnrichmentService, DataEnrichmentService>();
        services.AddScoped<IArtistDiscoveryService, ArtistDiscoveryService>();
        services.AddScoped<IRecommendationScoringService, RecommendationScoringService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISpotifyService, SpotifyService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IBandService, BandService>();
        services.AddScoped<IMusicDiscoveryService, MusicDiscoveryService>();
        services.AddScoped<IMusicEventService, MusicEventService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IForumService, ForumService>();
        services.AddScoped<IMerchandiseService, MerchandiseService>();
        services.AddScoped<IFollowService, FollowService>();
        services.AddScoped<IBandFollowService, BandFollowService>();
        services.AddScoped<IBandChatService, BandChatService>();
        services.AddScoped<IBandMessageService, BandMessageService>();

        return services;
    }

    public static IServiceCollection AddApplicationBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<RefreshTokenCleanupService>();

        return services;
    }
}
