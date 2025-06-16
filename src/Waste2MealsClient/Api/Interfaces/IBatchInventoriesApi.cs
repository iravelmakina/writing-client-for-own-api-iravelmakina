using Waste2MealsClient.Models.Filters;
using Waste2MealsClient.Models.Requests;
using Waste2MealsClient.Models.Responses;

namespace Waste2MealsClient.Api.Interfaces;

public interface IBatchInventoriesApi
{
    Task<IEnumerable<BatchInventoryModel>> GetBatchInventoriesAsync(BatchInventoryFilter filter);
    Task<BatchInventoryModel> GetBatchInventoryByIdAsync(int id);
    Task<BatchInventoryModel> CreateBatchInventoryAsync(CreateBatchInventoryRequest request);
    Task<BatchInventoryModel> UpdateBatchInventoryAsync(UpdateBatchInventoryRequest request);
    Task DeleteBatchInventoryAsync(int id);
}