namespace Waste2MealsClient.Models.Requests;

public class UpdateBatchDefinitionRequest
{
    public int Id { get; set; }
    public Guid VendorId { get; set; }
    public string? Description { get; set; }
    public string? Tag { get; set; }
    public string? ImageUrl { get; set; }
    public double OriginalPrice { get; set; }
    public double DiscountPrice { get; set; }
    public TimeSpan PickupStartTime { get; set; }
    public TimeSpan PickupEndTime { get; set; }
}