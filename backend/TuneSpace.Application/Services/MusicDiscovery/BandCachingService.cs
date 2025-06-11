using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TuneSpace.Core.Interfaces.IInfrastructure;

namespace TuneSpace.Application.Services.MusicDiscovery;

internal class BandCachingService(
    IMemoryCache cache,
    ILogger<BandCachingService> logger) : IBandCachingService
{
    private readonly IMemoryCache _cache = cache;
    private readonly ILogger<BandCachingService> _logger = logger;

    bool IBandCachingService.TryGetCachedItem<T>(string cacheKey, out T? value) where T : class
    {
        bool cacheHit = _cache.TryGetValue(cacheKey, out value);

        _logger.LogDebug("Cache {Result} for registered bands lookup", cacheHit ? "hit" : "miss");

        return cacheHit;
    }

    void IBandCachingService.CacheItem<T>(string cacheKey, T item, TimeSpan duration)
    {
        _logger.LogDebug("Caching item for user query. Duration: {Duration}", duration);
        _cache.Set(cacheKey, item, duration);
    }

    T IBandCachingService.GetOrCreateCachedItem<T>(string cacheKey, Func<T> itemFactory, TimeSpan duration) where T : class
    {
        if (!_cache.TryGetValue(cacheKey, out T? result))
        {
            _logger.LogDebug("Cache miss");

            result = itemFactory();
            _cache.Set(cacheKey, result, duration);

            _logger.LogDebug("Added new item to cache");
        }
        else
        {
            _logger.LogDebug("Cache hit");
        }

        return result!;
    }

    async Task<T> IBandCachingService.GetOrCreateCachedItemAsync<T>(string cacheKey, Func<Task<T>> itemFactory, TimeSpan duration) where T : class
    {
        if (!_cache.TryGetValue(cacheKey, out T? result))
        {
            _logger.LogDebug("Cache miss");
            result = await itemFactory();
            _cache.Set(cacheKey, result, duration);
            _logger.LogDebug("Added new item to cache");
        }
        else
        {
            _logger.LogDebug("Cache hit");
        }

        return result!;
    }
}
