namespace MyAPI.Application.DTOs.Catalog.ProductVariant.Responses
{
    public class ProductVariantResponse
    {
        public int VariantId { get; set; }
        public int ProductId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string? Weight { get; set; }
        public string? GripSize { get; set; }
        public string? Color { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
    }
}
