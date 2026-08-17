namespace MyAPI.Application.DTOs.Catalog.ProductVariant.Responses
{
    public class LowStockVariantResponse
    {
        public int VariantId { get; set; }
        public int ProductId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }
}
