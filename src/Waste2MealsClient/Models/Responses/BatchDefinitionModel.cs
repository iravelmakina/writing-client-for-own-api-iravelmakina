namespace Waste2MealsClient.Models.Responses;

public record BatchDefinitionModel(
    int Id,
    Guid VendorId,
    string Description,
    string Tag,
    string ImageUrl,
    decimal OriginalPrice,
    decimal DiscountPrice,
    TimeSpan PickupStartTime,
    TimeSpan PickupEndTime
);