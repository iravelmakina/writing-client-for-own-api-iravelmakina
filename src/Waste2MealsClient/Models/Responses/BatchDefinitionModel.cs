namespace Waste2MealsClient.Models.Responses;

public record BatchDefinitionModel(
    int Id,
    Guid VendorId,
    string Description,
    string Tag,
    string ImageUrl,
    double OriginalPrice,
    double DiscountPrice,
    TimeSpan PickupStartTime,
    TimeSpan PickupEndTime
);