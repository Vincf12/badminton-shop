using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Order.Cart.Responses
{
    public class CartDto
    {
        public int CartId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public decimal TotalAmount => Items.Sum(x => x.SubTotal);
    }

    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public int VariantId { get; set; }
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }

        public string Sku { get; set; } = string.Empty;
        public string? Weight { get; set; }
        public string? GripSize { get; set; }
        public string? Color { get; set; }

        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int StockQuantity { get; set; }

        public decimal SubTotal => Price * Quantity;
    }
}
