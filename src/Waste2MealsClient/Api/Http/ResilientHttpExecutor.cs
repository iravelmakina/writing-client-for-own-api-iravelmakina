using System.Net.Http.Json;
using System.Text.Json;
using Polly;

namespace Waste2MealsClient.Api.Http;

public class ResilientHttpExecutor
{
    private readonly HttpClient _httpClient;
    private readonly IAsyncPolicy<HttpResponseMessage> _resiliencePolicy;
    private readonly JsonSerializerOptions _serializerOptions;

    public ResilientHttpExecutor(
        HttpClient httpClient,
        int defaultRetryCount = 3,
        int defaultRetryDelayMs = 500,
        int defaultTimeoutMs = 30000,
        IAsyncPolicy<HttpResponseMessage>? resiliencePolicy = null,
        JsonSerializerOptions? serializerOptions = null)
    {
        _httpClient = httpClient;
        _resiliencePolicy = resiliencePolicy ?? ResiliencePolicyFactory.Create(
            defaultRetryCount,
            defaultRetryDelayMs,
            defaultTimeoutMs
        );
        _serializerOptions = serializerOptions ?? new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
    
    public IAsyncPolicy<HttpResponseMessage> ResiliencePolicy => _resiliencePolicy;

    public async Task<T> ExecuteWithPolicyAsync<T>(Func<HttpClient, Task<HttpResponseMessage>> requestFunc)
    {
        var response = await _resiliencePolicy.ExecuteAsync(async () =>
        {
            var httpResponse = await requestFunc(_httpClient);
            httpResponse.EnsureSuccessStatusCode();
            return httpResponse;
        });

        return await response.Content.ReadFromJsonAsync<T>(_serializerOptions)
               ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    public async Task ExecuteWithPolicyAsync(Func<HttpClient, Task<HttpResponseMessage>> requestFunc)
    {
        await _resiliencePolicy.ExecuteAsync(async () =>
        {
            var httpResponse = await requestFunc(_httpClient);
            httpResponse.EnsureSuccessStatusCode();
            return httpResponse;
        });
    }
}