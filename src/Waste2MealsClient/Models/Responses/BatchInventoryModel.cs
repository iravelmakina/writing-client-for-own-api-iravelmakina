namespace Waste2MealsClient.Models.Responses;

public record BatchInventoryModel(
    int Id,
    int BatchDefinitionId,
    int AvailableQuantity,
    string Status,
    DateTime ExpiryDate
);