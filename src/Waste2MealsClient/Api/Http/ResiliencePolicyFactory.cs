using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace Waste2MealsClient.Api.Http;

public static class ResiliencePolicyFactory
{
    public static IAsyncPolicy<HttpResponseMessage> Create(int retryCount, int retryAfterMs, int timeoutMs)
    {
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                retryCount,
                attempt => TimeSpan.FromMilliseconds(attempt * retryAfterMs));

        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(timeoutMs));
        return Policy.WrapAsync(retryPolicy, timeoutPolicy);
    }
}