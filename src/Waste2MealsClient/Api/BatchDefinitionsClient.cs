using System.Net;
using System.Net.Http.Json;
using Waste2MealsClient.Api.Http;
using Waste2MealsClient.Api.Interfaces;
using Waste2MealsClient.Models.Filters;
using Waste2MealsClient.Models.Requests;
using Waste2MealsClient.Models.Responses;
namespace Waste2MealsClient.Api;

public class BatchDefinitionsClient : IBatchDefinitionsClient
{
    private readonly ResilientHttpExecutor _httpExecutor;

    public BatchDefinitionsClient(HttpClient httpClient)
    {
        _httpExecutor = new ResilientHttpExecutor(httpClient);
    }

    public async Task<List<BatchDefinitionModel>> GetBatchDefinitionsAsync(int? pageSize = null, int? pageNumber = null, Guid? vendorId = null,
        string? tag = null, decimal? minPrice = null, decimal? maxPrice = null, TimeOnly? pickupAfter = null, TimeOnly? pickupBefore = null)
    {
        var filter = new BatchDefinitionFilter
        {
            PageSize = pageSize,
            PageNumber = pageNumber,
            VendorId = vendorId,
            Tag = tag,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            PickupAfter = pickupAfter,
            PickupBefore = pickupBefore
        };
        
        var queryString = BuildQueryString(filter);
        return await _httpExecutor.ExecuteWithPolicyAsync<List<BatchDefinitionModel>>(
            client => client.GetAsync($"batch_definitions{queryString}")
        );
    }

    public async Task<BatchDefinitionModel> GetBatchDefinitionByIdAsync(int id)
    {
        return await _httpExecutor.ExecuteWithPolicyAsync<BatchDefinitionModel>(
            client => client.GetAsync($"batch_definitions/{id}")
        );
    }

    public async Task<BatchDefinitionModel> CreateBatchDefinitionAsync(CreateBatchDefinitionRequest request)
    {
        return await _httpExecutor.ExecuteWithPolicyAsync<BatchDefinitionModel>(
            async client =>
            {
                var response = await client.PostAsJsonAsync("batch_definitions", request);
                return response;
            }
        );
    }

    public async Task<BatchDefinitionModel> UpdateBatchDefinitionAsync(UpdateBatchDefinitionRequest request)
    {
        return await _httpExecutor.ExecuteWithPolicyAsync<BatchDefinitionModel>(
            async client =>
            {
                var response = await client.PutAsJsonAsync("batch_definitions", request);
                return response;
            }
        );
    }

    public async Task DeleteBatchDefinitionAsync(int id)
    {
        await _httpExecutor.ExecuteWithPolicyAsync(
            client => client.DeleteAsync($"batch_definitions/{id}")
        );
    }

    public string BuildQueryString(BatchDefinitionFilter filter)
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
            queryParams.Add($"pickupAfter={filter.PickupAfter.Value.ToString("HH\\:mm\\:ss")}");

        if (filter.PickupBefore.HasValue)
            queryParams.Add($"pickupBefore={filter.PickupBefore.Value.ToString("HH\\:mm\\:ss")}");

        if (filter.PageNumber.HasValue)
            queryParams.Add($"pageNumber={filter.PageNumber}");

        if (filter.PageSize.HasValue)
            queryParams.Add($"pageSize={filter.PageSize}");

        return queryParams.Count > 0 ? $"?{string.Join("&", queryParams)}" : string.Empty;
    }
}