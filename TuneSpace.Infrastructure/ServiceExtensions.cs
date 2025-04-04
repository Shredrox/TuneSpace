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

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TuneSpaceDbContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("TuneSpaceDb")));
        
        services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme);
        services.AddAuthorizationBuilder();

        services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<TuneSpaceDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBandRepository, BandRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ITagRepository, TagRepository>();

        services.AddHttpClient<IOllamaClient, OllamaClient>();
        services.AddHttpClient<ILastFmClient, LastFmClient>();
        services.AddHttpClient<IMusicBrainzClient, MusicBrainzClient>();
        services.AddHttpClient<ISpotifyClient, SpotifyClient>();

        services.AddSignalR();

        return services;
    }
}