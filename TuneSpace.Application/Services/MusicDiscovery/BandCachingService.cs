using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TuneSpace.Core.Interfaces.IInfrastructure;

namespace TuneSpace.Application.Services.MusicDiscovery;

internal class BandCachingService : IBandCachingService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<BandCachingService> _logger;

    public BandCachingService(IMemoryCache cache, ILogger<BandCachingService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    bool IBandCachingService.TryGetCachedItem<T>(string cacheKey, out T? value) where T : class
    {
        return _cache.TryGetValue(cacheKey, out value);
    }

    void IBandCachingService.CacheItem<T>(string cacheKey, T item, TimeSpan duration)
    {
        _cache.Set(cacheKey, item, duration);
    }

    T IBandCachingService.GetOrCreateCachedItem<T>(string cacheKey, Func<T> itemFactory, TimeSpan duration) where T : class
    {
        if (!_cache.TryGetValue(cacheKey, out T? result))
        {
            result = itemFactory();
            _cache.Set(cacheKey, result, duration);
        }

        return result!;
    }

    async Task<T> IBandCachingService.GetOrCreateCachedItemAsync<T>(string cacheKey, Func<Task<T>> itemFactory, TimeSpan duration) where T : class
    {
        if (!_cache.TryGetValue(cacheKey, out T? result))
        {
            result = await itemFactory();
            _cache.Set(cacheKey, result, duration);
        }

        return result!;
    }
}
