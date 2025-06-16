using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Waste2MealsClient.Api.Interfaces;
using Waste2MealsClient.Models.Filters;
using Waste2MealsClient.Models.Requests;
using Waste2MealsClient.Models.Responses;

namespace Waste2MealsClient.Api;

public class BatchInventoriesClient : IBatchInventoriesClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerOptions;

    public BatchInventoriesClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _serializerOptions = new JsonSerializerOptions
            
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<IEnumerable<BatchInventoryModel>> GetBatchInventoriesAsync(
        BatchInventoryFilter? filter = null)
    {
        var queryString = BuildQueryString(filter);
        var response = await _httpClient.GetAsync($"batch_inventories{queryString}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<BatchInventoryModel>>(_serializerOptions)
               ?? throw new InvalidOperationException("Failed to deserialize response content.");
    }

    public async Task<BatchInventoryModel> GetBatchInventoryByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"batch_inventories/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BatchInventoryModel>(_serializerOptions) ??
               throw new InvalidOperationException("Failed to deserialize response content.");
    }

    public async Task<BatchInventoryModel> CreateBatchInventoryAsync(
        CreateBatchInventoryRequest request)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(request, _serializerOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync("batch_inventories", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BatchInventoryModel>(_serializerOptions) ??
               throw new InvalidOperationException("Failed to deserialize response content.");
    }

    public async Task<BatchInventoryModel> UpdateBatchInventoryAsync(
        UpdateBatchInventoryRequest request)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(request, _serializerOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PutAsync("batch_inventories", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BatchInventoryModel>(_serializerOptions) ??
               throw new InvalidOperationException("Failed to deserialize response content.");
    }

    public async Task DeleteBatchInventoryAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"batch_inventories/{id}");
        response.EnsureSuccessStatusCode();
    }

    private string BuildQueryString(BatchInventoryFilter? filter)
    {
        if (filter == null) return string.Empty;

        var queryParams = new List<string>();

        if (!string.IsNullOrEmpty(filter.Status))
            queryParams.Add($"status={WebUtility.UrlEncode(filter.Status)}");

        if (filter.MinQuantity.HasValue)
            queryParams.Add($"minQuantity={filter.MinQuantity}");

        if (filter.MaxQuantity.HasValue)
            queryParams.Add($"maxQuantity={filter.MaxQuantity}");
        
        if (filter.ExpireAfter.HasValue)
            queryParams.Add($"expireAfter={filter.ExpireAfter.Value:o}");

        if (filter.ExpireBefore.HasValue)
            queryParams.Add($"expireBefore={filter.ExpireBefore.Value:o}");

        if (filter.PageNumber.HasValue)
            queryParams.Add($"pageNumber={filter.PageNumber}");

        if (filter.PageSize.HasValue)
            queryParams.Add($"pageSize={filter.PageSize}");

        return queryParams.Count > 0 ? $"?{string.Join("&", queryParams)}" : string.Empty;
    }
}