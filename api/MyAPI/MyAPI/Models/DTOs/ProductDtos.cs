using System.ComponentModel.DataAnnotations;

namespace MyAPI.Models.DTOs
{
    public class ProductListItemDto
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsInStock => Stock > 0;
    }

    public class ProductDetailDto : ProductListItemDto
    {
        public DateTime? UpdatedAt { get; set; }
    }

    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class ProductUpsertDto
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(150)]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Brand { get; set; }

        [Range(typeof(decimal), "0", "999999999999")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        public string? Description { get; set; }
    }
}