namespace TuneSpace.Core.Interfaces.IInfrastructure;

/// <summary>
/// Service for throttling API calls to prevent hitting rate limits.
/// </summary>
public interface IApiThrottler
{
    /// <summary>
    /// Executes an API call with throttling to limit concurrent requests.
    /// </summary>
    /// <typeparam name="T">The return type of the API call.</typeparam>
    /// <param name="apiCall">The function representing the API call to execute.</param>
    /// <returns>The result of the API call.</returns>
    Task<T> ThrottledApiCall<T>(Func<Task<T>> apiCall);
}
