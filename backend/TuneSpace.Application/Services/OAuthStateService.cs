using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TuneSpace.Application.Common;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Application.Services;

internal class OAuthStateService(
    IMemoryCache cache,
    ILogger<OAuthStateService> logger) : IOAuthStateService
{
    private readonly IMemoryCache _cache = cache;
    private readonly ILogger<OAuthStateService> _logger = logger;

    private const string StateKeyPrefix = "oauth_state_";
    private static readonly TimeSpan StateExpiration = TimeSpan.FromMinutes(10);

    string IOAuthStateService.GenerateAndStoreState()
    {
        var state = Helpers.GenerateRandomString(32);
        var cacheKey = $"{StateKeyPrefix}{state}";

        _cache.Set(cacheKey, true, StateExpiration);

        _logger.LogDebug("Generated and stored OAuth state");

        return state;
    }

    bool IOAuthStateService.ValidateAndConsumeState(string state)
    {
        if (string.IsNullOrEmpty(state))
        {
            _logger.LogWarning("OAuth state validation failed: state is null or empty");
            return false;
        }

        var cacheKey = $"{StateKeyPrefix}{state}";

        if (_cache.TryGetValue(cacheKey, out _))
        {
            _cache.Remove(cacheKey);

            _logger.LogDebug("OAuth state validation successful");
            return true;
        }

        _logger.LogWarning("OAuth state validation failed");

        return false;
    }
}
