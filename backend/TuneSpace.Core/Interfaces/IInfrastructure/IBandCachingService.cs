namespace TuneSpace.Core.Interfaces.IInfrastructure;

/// <summary>
/// Service for caching band-related data to improve performance and reduce API calls.
/// </summary>
public interface IBandCachingService
{
    /// <summary>
    /// Tries to get an item from the cache.
    /// </summary>
    /// <typeparam name="T">The type of the cached item.</typeparam>
    /// <param name="cacheKey">The key used to identify the cached item.</param>
    /// <param name="value">When this method returns, contains the cached value if found, or null if not found.</param>
    /// <returns>True if the item was found in the cache; otherwise, false.</returns>
    bool TryGetCachedItem<T>(string cacheKey, out T? value) where T : class;

    /// <summary>
    /// Adds or updates an item in the cache with a specified expiration duration.
    /// </summary>
    /// <typeparam name="T">The type of the item to cache.</typeparam>
    /// <param name="cacheKey">The key to use for storing the item.</param>
    /// <param name="item">The item to store in the cache.</param>
    /// <param name="duration">The duration after which the item should expire from the cache.</param>
    void CacheItem<T>(string cacheKey, T item, TimeSpan duration);

    /// <summary>
    /// Gets an item from the cache or creates and adds it if not found.
    /// </summary>
    /// <typeparam name="T">The type of the cached item.</typeparam>
    /// <param name="cacheKey">The key used to identify the cached item.</param>
    /// <param name="itemFactory">A factory function to create the item if not found in the cache.</param>
    /// <param name="duration">The duration after which the item should expire from the cache.</param>
    /// <returns>The cached or newly created item.</returns>
    T GetOrCreateCachedItem<T>(string cacheKey, Func<T> itemFactory, TimeSpan duration) where T : class;

    /// <summary>
    /// Gets an item from the cache or asynchronously creates and adds it if not found.
    /// </summary>
    /// <typeparam name="T">The type of the cached item.</typeparam>
    /// <param name="cacheKey">The key used to identify the cached item.</param>
    /// <param name="itemFactory">An asynchronous factory function to create the item if not found in the cache.</param>
    /// <param name="duration">The duration after which the item should expire from the cache.</param>
    /// <returns>The cached or newly created item.</returns>
    Task<T> GetOrCreateCachedItemAsync<T>(string cacheKey, Func<Task<T>> itemFactory, TimeSpan duration) where T : class;
}
