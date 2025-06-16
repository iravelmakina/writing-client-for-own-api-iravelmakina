namespace Waste2MealsClient.Models.Responses;

public class BatchDefinition
{
    public int Id { get; set; }
    public Guid VendorId { get; set; }
    public required string Description { get; set; }
    public required string Tag { get; set; }
    public required string ImageUrl { get; set; }
    public double OriginalPrice { get; set; }
    public double DiscountPrice { get; set; }
    public TimeSpan PickupStartTime { get; set; }
    public TimeSpan PickupEndTime { get; set; }
}