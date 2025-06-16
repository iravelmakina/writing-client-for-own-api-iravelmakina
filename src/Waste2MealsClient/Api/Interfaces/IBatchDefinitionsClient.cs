using Waste2MealsClient.Models.Filters;
using Waste2MealsClient.Models.Requests;
using Waste2MealsClient.Models.Responses;

namespace Waste2MealsClient.Api.Interfaces;

public interface IBatchDefinitionsClient
{
    Task<List<BatchDefinitionModel>> GetBatchDefinitionsAsync(int? pageSize = null, int? pageNumber = null, Guid? vendorId = null,
        string? tag = null, decimal? minPrice = null, decimal? maxPrice = null, TimeOnly? pickupAfter = null, TimeOnly? pickupBefore = null);
    Task<BatchDefinitionModel> GetBatchDefinitionByIdAsync(int id);
    Task<BatchDefinitionModel> CreateBatchDefinitionAsync(CreateBatchDefinitionRequest request);
    Task<BatchDefinitionModel> UpdateBatchDefinitionAsync(UpdateBatchDefinitionRequest request);
    Task DeleteBatchDefinitionAsync(int id);
}