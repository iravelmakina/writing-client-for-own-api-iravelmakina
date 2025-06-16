using Waste2MealsClient.Models.Filters;
using Waste2MealsClient.Models.Requests;
using Waste2MealsClient.Models.Responses;

namespace Waste2MealsClient.Api.Interfaces;

public interface IBatchDefinitionsApi
{
    Task<IEnumerable<BatchDefinitionModel>> GetBatchDefinitionsAsync(BatchDefinitionFilter filter);
    Task<BatchDefinitionModel> GetBatchDefinitionByIdAsync(int id);
    Task<BatchDefinitionModel> CreateBatchDefinitionAsync(CreateBatchDefinitionRequest request);
    Task<BatchDefinitionModel> UpdateBatchDefinitionAsync(UpdateBatchDefinitionRequest request);
    Task DeleteBatchDefinitionAsync(int id);
}