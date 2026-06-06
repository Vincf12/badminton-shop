using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Models
{
    [Table("product_images")]
    public class ProductImage
    {
        [Key]
        [Column("image_id")]
        public int ImageId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("image_url")]
        [StringLength(255)]
        public string ImageUrl { get; set; } = string.Empty;

        [Column("is_main")]
        public bool IsMain { get; set; }

        [Column("sort_order")]
        public int SortOrder { get; set; }

        public Product Product { get; set; } = null!;
    }
}