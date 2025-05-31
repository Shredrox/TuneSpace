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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TuneSpace.Infrastructure.Identity;
using TuneSpace.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace TuneSpace.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
    {
        services.AddDbContext<TuneSpaceDbContext>((provider, options) =>
            options.UseNpgsql(provider.GetRequiredService<IOptionsSnapshot<DatabaseOptions>>().Value.DefaultConnection));

        return services;
    }

    public static IServiceCollection AddCachingServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        return services;
    }

    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        var jwtOptions = services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<JwtOptions>>().Value;

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (context.Request.Cookies.ContainsKey("AccessToken"))
                    {
                        context.Token = context.Request.Cookies["AccessToken"];
                    }

                    return Task.CompletedTask;
                }
            };
        }).AddCookie(IdentityConstants.ApplicationScheme);

        services.AddAuthorizationBuilder();

        services.AddIdentityCore<User>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
        }).AddRoles<ApplicationRole>()
        .AddEntityFrameworkStores<TuneSpaceDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBandRepository, BandRepository>();
        services.AddScoped<IMusicEventRepository, MusicEventRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IForumRepository, ForumRepository>();
        services.AddScoped<IMerchandiseRepository, MerchandiseRepository>();
        services.AddScoped<IFollowRepository, FollowRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

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

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.Configure<DatabaseOptions>(configuration.GetSection("ConnectionStrings"));
        services.Configure<SpotifyOptions>(configuration.GetSection("Spotify"));
        services.Configure<LastFmOptions>(configuration.GetSection("LastFm"));

        return services;
    }
}
