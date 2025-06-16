namespace Waste2MealsClient.Models.Responses;

public class BatchInventory
{
    public int Id { get; set; }
    public int BatchDefinitionId { get; set; }
    public int AvailableQuantity { get; set; }
    public required string Status { get; set; }
    public DateTime ExpiryDate { get; set; }
}