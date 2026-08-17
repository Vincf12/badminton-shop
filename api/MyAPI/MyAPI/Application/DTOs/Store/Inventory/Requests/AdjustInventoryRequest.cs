namespace MyAPI.Application.DTOs.Store.Inventory.Requests;

public class AdjustInventoryRequest
{
    public int VariantId { get; set; }

    /// <summary>
    /// + : nhập
    /// - : xuất
    /// </summary>
    public int Quantity { get; set; }

    public string Reason { get; set; } = string.Empty;
}
