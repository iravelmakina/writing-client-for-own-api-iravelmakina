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

public class BatchInventoriesClientTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly BatchInventoriesClient _client;

    public BatchInventoriesClientTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };
        _client = new BatchInventoriesClient(_httpClient);
    }

    [Fact]
    public async Task GetBatchInventoriesAsync_ShouldReturnBatchInventories()
    {
        // Arrange
        var expectedInventories = new List<BatchInventoryModel>
        {
            new(1, 1, 2, "ACTIVE", DateTime.Now.AddDays(7)),
            new(2, 2, 3, "INACTIVE", DateTime.Now.AddDays(14))
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.ToString().EndsWith("batch_inventories")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedInventories), Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _client.GetBatchInventoriesAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal(expectedInventories.First().Id, result.First().Id);
        Assert.Equal(expectedInventories.Last().Status, result.Last().Status);
    }

    [Fact]
    public async Task GetBatchInventoriesAsync_WithFilters_ShouldBuildCorrectQueryString()
    {
        // Arrange
        var expectedUrl = "/batch_inventories?status=ACTIVE&minQuantity=5&maxQuantity=10&expireAfter=2023-01-01T00:00:00.0000000&expireBefore=2023-12-31T00:00:00.0000000&pageNumber=2&pageSize=5";

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
        var result = await _client.GetBatchInventoriesAsync(
            pageSize: 5,
            pageNumber: 2,
            status: "ACTIVE",
            minQuantity: 5,
            maxQuantity: 10,
            expireAfter: new DateTime(2023, 1, 1),
            expireBefore: new DateTime(2023, 12, 31));

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBatchInventoryByIdAsync_ShouldReturnSingleInventory()
    {
        // Arrange
        var expectedInventory = new BatchInventoryModel(1, 1, 2, "ACTIVE", DateTime.Now.AddDays(7));

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.ToString().EndsWith("batch_inventories/1")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedInventory), Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _client.GetBatchInventoryByIdAsync(1);

        // Assert
        Assert.Equal(expectedInventory.Id, result.Id);
        Assert.Equal(expectedInventory.Status, result.Status);
    }

    [Fact]
    public async Task GetBatchInventoryByIdAsync_WhenNotFound_ShouldThrow()
    {
        // Arrange
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.ToString().EndsWith("batch_inventories/999")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => 
            _client.GetBatchInventoryByIdAsync(999));
    }

    [Fact]
    public async Task CreateBatchInventoryAsync_ShouldReturnCreatedInventory()
    {
        // Arrange
        var request = new CreateBatchInventoryRequest(
            1,
            2, 
            "ACTIVE", 
            DateTime.Now.AddDays(10));

        var expectedResponse = new BatchInventoryModel(1, request.BatchDefinitionId, request.AvailableQuantity, request.Status, request.ExpiryDate);

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.ToString().EndsWith("batch_inventories")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Created,
                Content = new StringContent(JsonSerializer.Serialize(expectedResponse), Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _client.CreateBatchInventoryAsync(request);

        // Assert
        Assert.Equal(expectedResponse.Id, result.Id);
        Assert.Equal(request.Status, result.Status);
    }
    
    [Fact]
    public async Task UpdateBatchInventoryAsync_ShouldReturnUpdatedInventory()
    {
        // Arrange
        var request = new UpdateBatchInventoryRequest(
            1,
            1,
            2, 
            "ACTIVE", 
            DateTime.Now.AddDays(10));

        var expectedResponse = new BatchInventoryModel(request.Id, request.BatchDefinitionId, request.AvailableQuantity,request.Status,  request.ExpiryDate);

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Put &&
                    req.RequestUri!.ToString().EndsWith("batch_inventories")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedResponse), Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _client.UpdateBatchInventoryAsync(request);

        // Assert
        Assert.Equal(request.Id, result.Id);
        Assert.Equal(request.Status, result.Status);
    }
    
    [Fact]
    public async Task UpdateBatchInventoryAsync_WhenNotFound_ShouldThrow()
    {
        // Arrange
        var request = new UpdateBatchInventoryRequest(
            999, 
            1,
            2, 
            "ACTIVE", 
            DateTime.Now.AddDays(10));

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Put &&
                    req.RequestUri!.ToString().EndsWith("batch_inventories")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => 
            _client.UpdateBatchInventoryAsync(request));
    }

    [Fact]
    public async Task DeleteBatchInventoryAsync_ShouldSucceed()
    {
        // Arrange
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Delete &&
                    req.RequestUri!.ToString().EndsWith("batch_inventories/1")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NoContent));

        // Act & Assert (should not throw)
        await _client.DeleteBatchInventoryAsync(1);
    }

    [Fact]
    public async Task DeleteBatchInventoryAsync_WhenNotFound_ShouldThrow()
    {
        // Arrange
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Delete &&
                    req.RequestUri!.ToString().EndsWith("batch_inventories/999")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => 
            _client.DeleteBatchInventoryAsync(999));
    }

    [Fact]
    public void BuildQueryString_WithAllFilters_ShouldReturnCorrectString()
    {
        // Arrange
        var filter = new BatchInventoryFilter
        {
            Status = "ACTIVE",
            MinQuantity = 5,
            MaxQuantity = 10,
            ExpireAfter = new DateTime(2023, 1, 1),
            ExpireBefore = new DateTime(2023, 12, 31),
            PageNumber = 2,
            PageSize = 5
        };

        // Act
        var queryString = _client.BuildQueryString(filter);

        // Assert
        Assert.Contains("status=ACTIVE", queryString);
        Assert.Contains("minQuantity=5", queryString);
        Assert.Contains("maxQuantity=10", queryString);
        Assert.Contains("expireAfter=2023-01-01", queryString);
        Assert.Contains("expireBefore=2023-12-31", queryString);
        Assert.Contains("pageNumber=2", queryString);
        Assert.Contains("pageSize=5", queryString);
    }

    [Fact]
    public void BuildQueryString_WithNoFilters_ShouldReturnEmptyString()
    {
        // Arrange
        var filter = new BatchInventoryFilter();

        // Act
        var result = _client.BuildQueryString(filter);

        // Assert
        Assert.Equal(string.Empty, result);
    }
}