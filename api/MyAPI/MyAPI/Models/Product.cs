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

        [Required]
        [Column("brand_id")]
        public int BrandId { get; set; }

        [Required]
        [Column("product_name")]
        [StringLength(150)]
        public string ProductName { get; set; } = string.Empty;

        [Column("slug")]
        [StringLength(255)]
        public string? Slug { get; set; }

        [Column("short_description")]
        public string? ShortDescription { get; set; }

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "active";

        [Column("description")]
        public string? Description { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public Category Category { get; set; } = null!;
        public Brand Brand { get; set; } = null!;
    }
}