namespace MyAPI.Application.DTOs.Store.Inventory.Requests;

public class UpdateInventoryRequest
{
    public int StockQuantity { get; set; }

    public int LowStockThreshold { get; set; }
}
