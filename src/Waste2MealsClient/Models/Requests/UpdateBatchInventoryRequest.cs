namespace Waste2MealsClient.Models;

public class UpdateBatchInventoryRequest
{
    public int Id { get; set; }
    public int BatchDefinitionId { get; set; }
    public int AvailableQuantity { get; set; }
    public string? Status { get; set; }
    public DateTime ExpiryDate { get; set; }
}