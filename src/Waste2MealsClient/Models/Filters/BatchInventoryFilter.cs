namespace Waste2MealsClient.Models.Filters;

public record BatchInventoryFilter(
    string? Status = null,
    int? MinQuantity = null,
    int? MaxQuantity = null,
    DateTime? ExpireAfter = null,
    DateTime? ExpireBefore = null,
    int PageNumber = 1,
    int PageSize = 10
);