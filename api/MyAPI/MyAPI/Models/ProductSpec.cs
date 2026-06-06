using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Models
{
    [Table("product_specs")]
    public class ProductSpec
    {
        [Key]
        [Column("spec_id")]
        public int SpecId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("spec_name")]
        [StringLength(100)]
        public string SpecName { get; set; } = string.Empty;

        [Column("spec_value")]
        [StringLength(255)]
        public string SpecValue { get; set; } = string.Empty;

        public Product Product { get; set; } = null!;
    }
}