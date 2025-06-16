namespace Waste2MealsClient.Models.Requests;

public class CreateBatchDefinitionRequest
{
    public Guid VendorId { get; set; }
    public string? Description { get; set; }
    public string? Tag { get; set; }
    public string? ImageUrl { get; set; }
    public double OriginalPrice { get; set; }
    public double DiscountPrice { get; set; }
    public TimeSpan PickupStartTime { get; set; }
    public TimeSpan PickupEndTime { get; set; }
}