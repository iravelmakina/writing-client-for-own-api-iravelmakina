namespace Waste2MealsClient.Models.Requests;

    public class CreateBatchInventoryRequest
{
    public int BatchDefinitionId { get; set; }
    public int AvailableQuantity { get; set; }
    public string? Status { get; set; }
    public DateTime ExpiryDate { get; set; }
}
