using Waste2MealsClient.Api.Http;
using Polly.Wrap;
using Xunit;

namespace Waste2MealsClient.Tests;

public class ResiliencePolicyFactoryTests
{
    [Fact]
    public void Create_ReturnsCombinedRetryAndTimeoutPolicy()
    {
        // Arrange
        const int retryCount = 3;
        const int retryAfterMs = 500;
        const int timeoutMs = 30000;

        // Act
        var policy = ResiliencePolicyFactory.Create(retryCount, retryAfterMs, timeoutMs);

        // Assert
        Assert.NotNull(policy);
        Assert.IsAssignableFrom<AsyncPolicyWrap<HttpResponseMessage>>(policy);
    }
}