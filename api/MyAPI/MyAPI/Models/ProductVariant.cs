using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Models
{
    [Table("product_variants")]
    public class ProductVariant
    {
        [Key]
        [Column("variant_id")]
        public int VariantId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("sku")]
        [StringLength(100)]
        public string Sku { get; set; } = string.Empty;

        [Column("weight")]
        [StringLength(50)]
        public string? Weight { get; set; }

        [Column("grip_size")]
        [StringLength(50)]
        public string? GripSize { get; set; }

        [Column("color")]
        [StringLength(100)]
        public string? Color { get; set; }

        [Column("price", TypeName = "decimal(12,2)")]
        public decimal Price { get; set; }

        [Column("stock_quantity")]
        public int StockQuantity { get; set; }

        [Column("image_url")]
        [StringLength(255)]
        public string? ImageUrl { get; set; }

        public Product Product { get; set; } = null!;
    }
}