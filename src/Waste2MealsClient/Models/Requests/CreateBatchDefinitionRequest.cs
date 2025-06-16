namespace Waste2MealsClient.Models.Requests;

public record CreateBatchDefinitionRequest(
    Guid VendorId,
    string Description,
    string Tag,
    string ImageUrl,
    decimal OriginalPrice,
    decimal DiscountPrice,
    TimeSpan PickupStartTime,
    TimeSpan PickupEndTime
);