using TuneSpace.Core.Interfaces.IInfrastructure;

namespace TuneSpace.Application.Services.MusicDiscovery;

internal class ApiThrottler : IApiThrottler
{
    private readonly SemaphoreSlim _semaphore;

    public ApiThrottler(int maxConcurrentRequests = 3)
    {
        _semaphore = new SemaphoreSlim(maxConcurrentRequests);
    }

    public async Task<T> ThrottledApiCall<T>(Func<Task<T>> apiCall)
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
