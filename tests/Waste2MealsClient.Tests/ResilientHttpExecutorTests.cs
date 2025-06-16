using Waste2MealsClient.Api.Http;
using System.Net;
using System.Text.Json;
using Moq;
using Moq.Protected;
using Polly;
using Xunit;

namespace Waste2MealsClient.Tests;

public class ResilientHttpExecutorTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly ResilientHttpExecutor _executor;
    private readonly JsonSerializerOptions _serializerOptions;

    public ResilientHttpExecutorTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _executor = new ResilientHttpExecutor(_httpClient, resiliencePolicy: Policy.NoOpAsync<HttpResponseMessage>());
    }

    [Fact]
    public async Task ExecuteWithPolicyAsync_WithResponse_ShouldReturnDeserializedObject()
    {
        // Arrange
        var expectedResponse = new TestModel(1, "Test Name");
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(expectedResponse, _serializerOptions))
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _executor.ExecuteWithPolicyAsync<TestModel>(client =>
            client.GetAsync("https://example.com/api/test"));

        // Assert
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(expectedResponse.Name, result.Name);
    }

    [Fact]
    public async Task ExecuteWithPolicyAsync_WithoutResponse_ShouldNotThrow()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act & Assert (should not throw)
        await _executor.ExecuteWithPolicyAsync(client =>
            client.DeleteAsync("https://example.com/api/test/1"));
    }

    [Fact]
    public async Task ExecuteWithPolicyAsync_WhenHttpError_ShouldThrow()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() =>
            _executor.ExecuteWithPolicyAsync(client =>
                client.GetAsync("https://example.com/api/test")));
    }

    [Fact]
    public async Task ExecuteWithPolicyAsync_WhenDeserializationFails_ShouldThrow()
    {
        // Arrange
        var invalidJson = "{ invalid: json }";
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(invalidJson)
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act & Assert
        await Assert.ThrowsAsync<JsonException >(() =>
            _executor.ExecuteWithPolicyAsync<TestModel>(client =>
                client.GetAsync("https://example.com/api/test")));
    }

    [Fact]
    public async Task ExecuteWithPolicyAsync_WithCustomPolicy_ShouldUseProvidedPolicy()
    {
        // Arrange
        var mockPolicy = new Mock<IAsyncPolicy<HttpResponseMessage>>();
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

        mockPolicy.Setup(p => p.ExecuteAsync(It.IsAny<Func<Task<HttpResponseMessage>>>()))
            .ReturnsAsync(responseMessage);

        var customExecutor = new ResilientHttpExecutor(
            _httpClient,
            resiliencePolicy: mockPolicy.Object);

        // Act
        await customExecutor.ExecuteWithPolicyAsync(client =>
            client.GetAsync("https://example.com/api/test"));

        // Assert
        mockPolicy.Verify(p => p.ExecuteAsync(
            It.IsAny<Func<Task<HttpResponseMessage>>>()), Times.Once);
    }
    [Fact]
    public void ResiliencePolicy_ShouldReturnConfiguredPolicy()
    {
        // Arrange
        var expectedPolicy = Policy.NoOpAsync<HttpResponseMessage>();
        var executor = new ResilientHttpExecutor(
            _httpClient,
            resiliencePolicy: expectedPolicy);

        // Act
        var policy = executor.ResiliencePolicy;

        // Assert
        Assert.Same(expectedPolicy, policy);
    }

    [Fact]
    public void Constructor_WithDefaultParameters_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var executor = new ResilientHttpExecutor(_httpClient);

        // Assert
        Assert.NotNull(executor.ResiliencePolicy);
        Assert.NotNull(executor);
    }

    [Fact]
    public void Constructor_WithCustomSerializerOptions_ShouldUseProvidedOptions()
    {
        // Arrange
        var customOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false
        };

        // Act
        var executor = new ResilientHttpExecutor(
            _httpClient,
            serializerOptions: customOptions);

        // Assert
        Assert.False(executor.ResiliencePolicy.GetType()
                         .GetField("_serializerOptions",
                             System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
                         .GetValue(executor.ResiliencePolicy) is JsonSerializerOptions options &&
                     options.PropertyNameCaseInsensitive);
    }

    private record TestModel(
        int Id,
        string Name
    );
}
