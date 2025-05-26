using TuneSpace.Core.Interfaces.IInfrastructure;

namespace TuneSpace.Application.Services.MusicDiscovery;

internal class ApiThrottler(int maxConcurrentRequests = 3) : IApiThrottler
{
    private readonly SemaphoreSlim _semaphore = new(maxConcurrentRequests);

    async Task<T> IApiThrottler.ThrottledApiCall<T>(Func<Task<T>> apiCall)
    {
        await _semaphore.WaitAsync();
        try
        {
            return await apiCall();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
