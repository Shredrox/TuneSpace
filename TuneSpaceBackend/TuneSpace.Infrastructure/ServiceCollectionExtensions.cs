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

    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt");

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
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings["Token"] ?? throw new InvalidOperationException("JWT Token secret is not configured"))
                ),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
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

        services.AddIdentityCore<User>()
            .AddRoles<ApplicationRole>()
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
        services.AddScoped<IMusicEventRepository, MusicEventRepository>();

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
