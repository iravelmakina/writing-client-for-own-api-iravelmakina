namespace Waste2MealsClient.Models.Requests;

public record CreateBatchDefinitionRequest(
    Guid VendorId,
    string Description,
    string Tag,
    string ImageUrl,
    double OriginalPrice,
    double DiscountPrice,
    TimeSpan PickupStartTime,
    TimeSpan PickupEndTime
);