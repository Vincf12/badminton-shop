namespace MyAPI.Application.DTOs.Store.Inventory.Requests;

public class CreateInventoryRequest
{
    public int VariantId { get; set; }

    public int StockQuantity { get; set; }

    public int LowStockThreshold { get; set; } = 5;
}
