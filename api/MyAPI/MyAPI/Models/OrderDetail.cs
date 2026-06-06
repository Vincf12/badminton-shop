using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Models
{
    [Table("order_details")]
    public class OrderDetail
    {
        [Key]
        [Column("order_detail_id")]
        public int OrderDetailId { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("variant_id")]
        public int VariantId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("unit_price", TypeName = "decimal(12,2)")]
        public decimal UnitPrice { get; set; }

        [Column("subtotal", TypeName = "decimal(12,2)")]
        public decimal SubTotal { get; set; }

        public Order Order { get; set; } = null!;
        public ProductVariant Variant { get; set; } = null!;
    }
}