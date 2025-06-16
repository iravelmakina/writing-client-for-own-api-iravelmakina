namespace Waste2MealsClient.Models.Requests;

public record UpdateBatchInventoryRequest(
    int Id,
    int BatchDefinitionId,
    int AvailableQuantity,
    string Status,
    DateTime ExpiryDate
);