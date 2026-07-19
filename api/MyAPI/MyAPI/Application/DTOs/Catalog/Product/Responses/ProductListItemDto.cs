namespace MyAPI.Application.DTOs.Catalog.Product.Responses
{
    public class ProductListItemDto
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsInStock => Stock > 0;
    }
}
