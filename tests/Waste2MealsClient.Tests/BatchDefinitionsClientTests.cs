using Waste2MealsClient.Api;
using Waste2MealsClient.Models.Filters;
using Waste2MealsClient.Models.Requests;
using Waste2MealsClient.Models.Responses;
using System.Net;
using System.Text;
using System.Text.Json;
using Moq;
using Moq.Protected;
using Xunit;

namespace Waste2MealsClient.Tests;

public class BatchDefinitionsClientTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly BatchDefinitionsClient _client;

    public BatchDefinitionsClientTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };
        _client = new BatchDefinitionsClient(_httpClient);
    }

    [Fact]
    public async Task GetBatchDefinitionsAsync_ShouldReturnBatchDefinitions()
    {
        // Arrange
        var expectedDefinitions = new List<BatchDefinitionModel>
        {
            new(1, Guid.NewGuid(), "Test 1", "tag1", "image1.jpg", 10.0m, 8.0m, TimeSpan.FromHours(10), TimeSpan.FromHours(12)),
            new(2, Guid.NewGuid(), "Test 2", "tag2", "image2.jpg", 15.0m, 12.0m, TimeSpan.FromHours(14), TimeSpan.FromHours(16))
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.ToString().EndsWith("batch_definitions")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedDefinitions), Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _client.GetBatchDefinitionsAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(expectedDefinitions.First().Id, result.First().Id);
        Assert.Equal(expectedDefinitions.Last().Description, result.Last().Description);
    }

    [Fact]
    public async Task GetBatchDefinitionsAsync_WithFilters_ShouldBuildCorrectQueryString()
    {
        // Arrange
        var expectedUrl = "/batch_definitions?vendorId=12345678-1234-1234-1234-123456789012&tag=test&minPrice=10&maxPrice=20&pickupAfter=10:00:00&pickupBefore=12:00:00&pageNumber=2&pageSize=5";

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri!.ToString().EndsWith(expectedUrl)),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[]", Encoding.UTF8, "application/json")
            });
        
        // Act
        var result = await _client.GetBatchDefinitionsAsync(
            pageSize: 5,
            pageNumber: 2,
            vendorId: Guid.Parse("12345678-1234-1234-1234-123456789012"),
            tag: "test",
            minPrice: 10,
            maxPrice: 20,
            pickupAfter: new TimeOnly(10, 0),
            pickupBefore: new TimeOnly(12, 0));

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBatchDefinitionByIdAsync_ShouldReturnSingleDefinition()
    {
        // Arrange
        var expectedDefinition = new BatchDefinitionModel(1, Guid.NewGuid(), "Test", "tag", "image.jpg", 10.0m, 8.0m, TimeSpan.FromHours(10), TimeSpan.FromHours(12));

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.ToString().EndsWith("batch_definitions/1")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedDefinition), Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _client.GetBatchDefinitionByIdAsync(1);

        // Assert
        Assert.Equal(expectedDefinition.Id, result.Id);
        Assert.Equal(expectedDefinition.Description, result.Description);
    }

    [Fact]
    public async Task GetBatchDefinitionByIdAsync_WhenNotFound_ShouldThrow()
    {
        // Arrange
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.ToString().EndsWith("batch_definitions/999")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => 
            _client.GetBatchDefinitionByIdAsync(999));
    }

    [Fact]
    public async Task CreateBatchDefinitionAsync_ShouldReturnCreatedDefinition()
    {
        // Arrange
        var request = new CreateBatchDefinitionRequest(
            Guid.NewGuid(), 
            "New Batch", 
            "new-tag", 
            "image.jpg", 
            15.0m, 
            12.0m, 
            TimeSpan.FromHours(9), 
            TimeSpan.FromHours(11));

        var expectedResponse = new BatchDefinitionModel(1, request.VendorId, request.Description, request.Tag, request.ImageUrl, request.OriginalPrice, request.DiscountPrice, request.PickupStartTime, request.PickupEndTime);

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.ToString().EndsWith("batch_definitions")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Created,
                Content = new StringContent(JsonSerializer.Serialize(expectedResponse), Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _client.CreateBatchDefinitionAsync(request);

        // Assert
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(request.Description, result.Description);
    }
    
    [Fact]
    public async Task CreateBatchDefinitionAsync_WhenVendorNotFound_ShouldThrow()
    {
        // Arrange
        var request = new CreateBatchDefinitionRequest(
            Guid.NewGuid(), 
            "New Batch", 
            "new-tag", 
            "image.jpg", 
            15.0m, 
            12.0m, 
            TimeSpan.FromHours(9), 
            TimeSpan.FromHours(11));

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.ToString().EndsWith("batch_definitions")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => 
            _client.CreateBatchDefinitionAsync(request));
    }

    [Fact]
    public async Task UpdateBatchDefinitionAsync_ShouldReturnUpdatedDefinition()
    {
        // Arrange
        var request = new UpdateBatchDefinitionRequest(
            1, 
            Guid.NewGuid(), 
            "Updated Batch", 
            "updated-tag", 
            "new-image.jpg", 
            18.0m, 
            15.0m, 
            TimeSpan.FromHours(10), 
            TimeSpan.FromHours(12));

        var expectedResponse = new BatchDefinitionModel(request.Id, request.VendorId, request.Description, request.Tag, request.ImageUrl, request.OriginalPrice, request.DiscountPrice, request.PickupStartTime, request.PickupEndTime);

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Put &&
                    req.RequestUri!.ToString().EndsWith("batch_definitions")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedResponse), Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _client.UpdateBatchDefinitionAsync(request);

        // Assert
        Assert.Equal(request.Id, result.Id);
        Assert.Equal(request.Description, result.Description);
    }
    
    [Fact]
    public async Task UpdateBatchDefinitionAsync_WhenNotFound_ShouldThrow()
    {
        // Arrange
        var request = new UpdateBatchDefinitionRequest(
            999, 
            Guid.NewGuid(), 
            "Updated Batch", 
            "updated-tag", 
            "new-image.jpg", 
            18.0m, 
            15.0m, 
            TimeSpan.FromHours(10), 
            TimeSpan.FromHours(12));

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Put &&
                    req.RequestUri!.ToString().EndsWith("batch_definitions")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => 
            _client.UpdateBatchDefinitionAsync(request));
    }

    [Fact]
    public async Task DeleteBatchDefinitionAsync_ShouldSucceed()
    {
        // Arrange
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Delete &&
                    req.RequestUri!.ToString().EndsWith("batch_definitions/1")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

        // Act & Assert (should not throw)
        await _client.DeleteBatchDefinitionAsync(1);
    }

    [Fact]
    public async Task DeleteBatchDefinitionAsync_WhenNotFound_ShouldThrow()
    {
        // Arrange
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Delete &&
                    req.RequestUri!.ToString().EndsWith("batch_definitions/999")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => 
            _client.DeleteBatchDefinitionAsync(999));
    }

    [Fact]
    public void BuildQueryString_WithAllFilters_ShouldReturnCorrectString()
    {
        // Arrange
        var filter = new BatchDefinitionFilter(
            VendorId: Guid.Parse("12345678-1234-1234-1234-123456789012"),
            Tag: "test",
            MinPrice: 10,
            MaxPrice: 20,
            PickupAfter: new TimeOnly(9, 0),
            PickupBefore: new TimeOnly(17, 0),
            PageNumber: 2,
            PageSize: 5
        );

        // Act
        var result = _client.BuildQueryString(filter);

        // Assert
        Assert.Contains("vendorId=12345678-1234-1234-1234-123456789012", result);
        Assert.Contains("tag=test", result);
        Assert.Contains("minPrice=10", result);
        Assert.Contains("maxPrice=20", result);
        Assert.Contains("pickupAfter=09:00:00", result);
        Assert.Contains("pickupBefore=17:00:00", result);
        Assert.Contains("pageNumber=2", result);
        Assert.Contains("pageSize=5", result);
    }

    [Fact]
    public void BuildQueryString_WithNoFilters_ShouldReturnEmptyString()
    {
        // Arrange
        var filter = new BatchDefinitionFilter();

        // Act
        var result = _client.BuildQueryString(filter);

        // Assert
        Assert.Equal(string.Empty, result);
    }
}