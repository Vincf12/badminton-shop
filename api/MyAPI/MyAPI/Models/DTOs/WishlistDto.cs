using System.ComponentModel.DataAnnotations;

namespace MyAPI.Models.DTOs
{
    public class WishlistItemDto
    {
        public int WishlistItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class WishlistDto
    {
        public int WishlistId { get; set; }
        public List<WishlistItemDto> Items { get; set; } = new();
    }

    public class AddWishlistItemDto
    {
        [Required]
        public int ProductId { get; set; }
    }
}
