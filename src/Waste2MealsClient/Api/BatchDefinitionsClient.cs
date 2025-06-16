using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Waste2MealsClient.Api.Interfaces;
using Waste2MealsClient.Models.Filters;
using Waste2MealsClient.Models.Requests;
using Waste2MealsClient.Models.Responses;

namespace Waste2MealsClient.Api;

public class BatchDefinitionsClient : IBatchDefinitionsClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerOptions;

    public BatchDefinitionsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<IEnumerable<BatchDefinitionModel>> GetBatchDefinitionsAsync(
        BatchDefinitionFilter filter)
    {
        var queryString = BuildQueryString(filter);
        var response = await _httpClient.GetAsync($"batch_definitions{queryString}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<BatchDefinitionModel>>(_serializerOptions)
               ?? throw new InvalidOperationException("Failed to deserialize response content.");
    }

    public async Task<BatchDefinitionModel> GetBatchDefinitionByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"batch_definitions/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BatchDefinitionModel>(_serializerOptions)
               ?? throw new InvalidOperationException("Failed to deserialize response content.");
    }

    public async Task<BatchDefinitionModel> CreateBatchDefinitionAsync(
        CreateBatchDefinitionRequest request)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(request, _serializerOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync("batch_definitions", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BatchDefinitionModel>(_serializerOptions)
               ?? throw new InvalidOperationException("Failed to deserialize response content.");
    }

    public async Task<BatchDefinitionModel> UpdateBatchDefinitionAsync(
        UpdateBatchDefinitionRequest request)
    {
        var content = new StringContent(
            JsonSerializer.Serialize(request, _serializerOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PutAsync("batch_definitions", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BatchDefinitionModel>(_serializerOptions)
               ?? throw new InvalidOperationException("Failed to deserialize response content.");
    }

    public async Task DeleteBatchDefinitionAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"batch_definitions/{id}");
        response.EnsureSuccessStatusCode();
    }

    private string BuildQueryString(BatchDefinitionFilter filter)
    {
        var queryParams = new List<string>();

        if (filter.VendorId.HasValue)
            queryParams.Add($"vendorId={filter.VendorId}");

        if (!string.IsNullOrEmpty(filter.Tag))
            queryParams.Add($"tag={WebUtility.UrlEncode(filter.Tag)}");

        if (filter.MinPrice.HasValue)
            queryParams.Add($"minPrice={filter.MinPrice}");

        if (filter.MaxPrice.HasValue)
            queryParams.Add($"maxPrice={filter.MaxPrice}");
        
        if (filter.PickupAfter.HasValue)
            queryParams.Add($"pickupAfter={filter.PickupAfter.Value.ToString("hh\\:mm\\:ss")}");

        if (filter.PickupBefore.HasValue)
            queryParams.Add($"pickupBefore={filter.PickupBefore.Value.ToString("hh\\:mm\\:ss")}");

        if (filter.PageNumber.HasValue)
            queryParams.Add($"pageNumber={filter.PageNumber}");

        if (filter.PageSize.HasValue)
            queryParams.Add($"pageSize={filter.PageSize}");

        return queryParams.Count > 0 ? $"?{string.Join("&", queryParams)}" : string.Empty;
    }
}