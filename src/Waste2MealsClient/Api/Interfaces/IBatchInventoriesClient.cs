using Waste2MealsClient.Models.Filters;
using Waste2MealsClient.Models.Requests;
using Waste2MealsClient.Models.Responses;

namespace Waste2MealsClient.Api.Interfaces;

public interface IBatchInventoriesClient
{
    Task<IEnumerable<BatchInventoryModel>> GetBatchInventoriesAsync(int? pageSize = null, int? pageNumber = null, string? status = null, int? minQuantity = null, int? maxQuantity = null, DateTime? expireAfter = null, DateTime? expireBefore = null); 
    Task<BatchInventoryModel> GetBatchInventoryByIdAsync(int id);
    Task<BatchInventoryModel> CreateBatchInventoryAsync(CreateBatchInventoryRequest request);
    Task<BatchInventoryModel> UpdateBatchInventoryAsync(UpdateBatchInventoryRequest request);
    Task DeleteBatchInventoryAsync(int id);
}