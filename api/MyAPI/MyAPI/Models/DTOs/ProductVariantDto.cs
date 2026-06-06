using System.ComponentModel.DataAnnotations;

namespace MyAPI.Models.DTOs
{
    public class ProductVariantDto
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

    public class ProductVariantUpsertDto
    {
        [Required]
        [StringLength(100)]
        public string Sku { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Weight { get; set; }

        [StringLength(50)]
        public string? GripSize { get; set; }

        [StringLength(100)]
        public string? Color { get; set; }

        [Range(typeof(decimal), "0", "999999999999")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [StringLength(255)]
        public string? ImageUrl { get; set; }
    }

    public class UpdateStockDto
    {
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
    }
}