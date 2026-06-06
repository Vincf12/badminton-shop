using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Models
{
    [Table("brands")]
    public class Brand
    {
        [Key]
        [Column("brand_id")]
        public int BrandId { get; set; }

        [Required]
        [Column("brand_name")]
        [StringLength(100)]
        public string BrandName { get; set; } = string.Empty;
    }
}