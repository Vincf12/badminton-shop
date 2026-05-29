using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Models
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("brand_id")]
        public int? BrandId { get; set; }

        [Column("supplier_id")]
        public int? SupplierId { get; set; }

        [Required]
        [Column("product_name")]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [Column("product_slug")]
        [StringLength(200)]
        public string ProductSlug { get; set; } = string.Empty;

        [Column("product_code")]
        [StringLength(100)]
        public string? ProductCode { get; set; }

        [Required]
        [Column("price", TypeName = "decimal(12,2)")]
        public decimal Price { get; set; }

        [Column("cost_price", TypeName = "decimal(12,2)")]
        public decimal? CostPrice { get; set; }

        [Column("discount_price", TypeName = "decimal(12,2)")]
        public decimal? DiscountPrice { get; set; }

        [Column("discount_percent")]
        public int? DiscountPercent { get; set; }

        [Column("discount_start_date", TypeName = "date")]
        public DateTime? DiscountStartDate { get; set; }

        [Column("discount_end_date", TypeName = "date")]
        public DateTime? DiscountEndDate { get; set; }

        [Column("total_stock")]
        public int? TotalStock { get; set; }

        [Column("low_stock_threshold")]
        public int? LowStockThreshold { get; set; }

        [Column("allow_backorder")]
        public bool? AllowBackorder { get; set; }

        [Column("meta_title")]
        [StringLength(200)]
        public string? MetaTitle { get; set; }

        [Column("meta_description")]
        public string? MetaDescription { get; set; }

        [Column("meta_keywords")]
        public string? MetaKeywords { get; set; }

        [Column("short_description")]
        [StringLength(500)]
        public string? ShortDescription { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("weight", TypeName = "decimal(8,2)")]
        public decimal? Weight { get; set; }

        [Column("dimensions")]
        [StringLength(50)]
        public string? Dimensions { get; set; }

        [Column("is_featured")]
        public bool? IsFeatured { get; set; }

        [Column("is_active")]
        public bool? IsActive { get; set; }

        [Column("is_new")]
        public bool? IsNew { get; set; }

        [Column("avg_rating", TypeName = "decimal(3,2)")]
        public decimal? AvgRating { get; set; }

        [Column("total_reviews")]
        public int? TotalReviews { get; set; }

        [Column("total_sold")]
        public int? TotalSold { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
