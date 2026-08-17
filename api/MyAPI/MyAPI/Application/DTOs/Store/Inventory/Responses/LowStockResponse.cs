namespace MyAPI.Application.DTOs.Store.Inventory.Responses;

public class LowStockResponse
{
    public int StoreId { get; set; }

    public string StoreName { get; set; } = string.Empty;

    public int VariantId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string VariantName { get; set; } = string.Empty;

    public int StockQuantity { get; set; }

    public int LowStockThreshold { get; set; }
}
