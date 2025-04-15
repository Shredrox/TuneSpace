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

        _logger.LogDebug("Cache {Result} for key: {CacheKey}",
            cacheHit ? "hit" : "miss",
            cacheKey);

        return cacheHit;
    }

    void IBandCachingService.CacheItem<T>(string cacheKey, T item, TimeSpan duration)
    {
        _logger.LogDebug("Caching item with key: {CacheKey} for duration: {Duration}", cacheKey, duration);
        _cache.Set(cacheKey, item, duration);
    }

    T IBandCachingService.GetOrCreateCachedItem<T>(string cacheKey, Func<T> itemFactory, TimeSpan duration) where T : class
    {
        if (!_cache.TryGetValue(cacheKey, out T? result))
        {
            _logger.LogDebug("Cache miss for key: {CacheKey}, creating new item", cacheKey);

            result = itemFactory();
            _cache.Set(cacheKey, result, duration);

            _logger.LogDebug("Added new item to cache with key: {CacheKey} for duration: {Duration}", cacheKey, duration);
        }
        else
        {
            _logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
        }

        return result!;
    }

    async Task<T> IBandCachingService.GetOrCreateCachedItemAsync<T>(string cacheKey, Func<Task<T>> itemFactory, TimeSpan duration) where T : class
    {
        if (!_cache.TryGetValue(cacheKey, out T? result))
        {
            _logger.LogDebug("Cache miss for key: {CacheKey}, creating new item asynchronously", cacheKey);
            result = await itemFactory();
            _cache.Set(cacheKey, result, duration);
            _logger.LogDebug("Added new item to cache with key: {CacheKey} for duration: {Duration}", cacheKey, duration);
        }
        else
        {
            _logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
        }

        return result!;
    }
}
