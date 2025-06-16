namespace Waste2MealsClient.Models.Filters;

public record BatchDefinitionFilter(
    Guid? VendorId = null,
    string? Tag = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    TimeOnly? PickupAfter = null,
    TimeOnly? PickupBefore = null,
    int? PageNumber = null,
    int? PageSize = null
);