using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Models
{
    [Table("cartitems")]
    public class CartItem
    {
        [Key]
        [Column("cart_item_id")]
        public int CartItemId { get; set; }

        [Column("cart_id")]
        public int CartId { get; set; }

        [Column("variant_id")]
        public int VariantId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        public Cart Cart { get; set; } = null!;
        public ProductVariant ProductVariant { get; set; } = null!;
    }
}