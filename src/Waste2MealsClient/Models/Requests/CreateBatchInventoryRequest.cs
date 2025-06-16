namespace Waste2MealsClient.Models.Requests;

public record CreateBatchInventoryRequest(
    int BatchDefinitionId,
    int AvailableQuantity,
    string Status,
    DateTime ExpiryDate
);