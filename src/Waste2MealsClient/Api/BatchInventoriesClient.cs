using System.Net;
using System.Net.Http.Json;
using Waste2MealsClient.Api.Http;
using Waste2MealsClient.Api.Interfaces;
using Waste2MealsClient.Models.Filters;
using Waste2MealsClient.Models.Requests;
using Waste2MealsClient.Models.Responses;

namespace Waste2MealsClient.Api;

public class BatchInventoriesClient : IBatchInventoriesClient
{
    private readonly ResilientHttpExecutor _httpExecutor;

    public BatchInventoriesClient(HttpClient httpClient)
    {
        _httpExecutor = new ResilientHttpExecutor(httpClient);
    }

    public async Task<List<BatchInventoryModel>> GetBatchInventoriesAsync(int? pageSize = null, int? pageNumber = null, string? status = null, int? minQuantity = null, int? maxQuantity = null, DateTime? expireAfter = null, DateTime? expireBefore = null)
    {
        var filter = new BatchInventoryFilter
        {
            PageSize = pageSize,
            PageNumber = pageNumber,
            Status = status,
            MinQuantity = minQuantity,
            MaxQuantity = maxQuantity,
            ExpireAfter = expireAfter,
            ExpireBefore = expireBefore
        };
        
        var queryString = BuildQueryString(filter);
        return await _httpExecutor.ExecuteWithPolicyAsync<List<BatchInventoryModel>>(
            client => client.GetAsync($"batch_inventories{queryString}")
        );
    }

    public async Task<BatchInventoryModel> GetBatchInventoryByIdAsync(int id)
    {
        return await _httpExecutor.ExecuteWithPolicyAsync<BatchInventoryModel>(
            client => client.GetAsync($"batch_inventories/{id}")
        );
    }

    public async Task<BatchInventoryModel> CreateBatchInventoryAsync(CreateBatchInventoryRequest request)
    {
        return await _httpExecutor.ExecuteWithPolicyAsync<BatchInventoryModel>(
            async client =>
            {
                var response = await client.PostAsJsonAsync("batch_inventories", request);
                return response;
            }
        );
    }

    public async Task<BatchInventoryModel> UpdateBatchInventoryAsync(UpdateBatchInventoryRequest request)
    {
        return await _httpExecutor.ExecuteWithPolicyAsync<BatchInventoryModel>(
            async client =>
            {
                var response = await client.PutAsJsonAsync("batch_inventories", request);
                return response;
            }
        );
    }

    public async Task DeleteBatchInventoryAsync(int id)
    {
        await _httpExecutor.ExecuteWithPolicyAsync(
            client => client.DeleteAsync($"batch_inventories/{id}")
        );
    }
    
    public string BuildQueryString(BatchInventoryFilter? filter)
    {
        var queryParams = new List<string>();

        if (!string.IsNullOrEmpty(filter!.Status))
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