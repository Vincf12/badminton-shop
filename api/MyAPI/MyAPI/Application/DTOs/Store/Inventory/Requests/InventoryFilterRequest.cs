namespace MyAPI.Application.DTOs.Store.Inventory.Requests;

public class InventoryFilterRequest
{
    public int? StoreId { get; set; }

    public int? VariantId { get; set; }

    public bool? LowStockOnly { get; set; }

    public string? Keyword { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
