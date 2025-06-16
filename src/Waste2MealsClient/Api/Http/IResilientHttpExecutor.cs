using Polly;

namespace Waste2MealsClient.Api.Http;

public interface IResilientHttpExecutor
{
    IAsyncPolicy<HttpResponseMessage> ResiliencePolicy { get; }
    Task<T> ExecuteWithPolicyAsync<T>(Func<HttpClient, Task<HttpResponseMessage>> requestFunc);
    Task ExecuteWithPolicyAsync(Func<HttpClient, Task<HttpResponseMessage>> requestFunc);
}
