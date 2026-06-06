using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Models
{
    [Table("wishlist_items")]
    public class WishlistItem
    {
        [Key]
        [Column("wishlist_item_id")]
        public int WishlistItemId { get; set; }

        [Column("wishlist_id")]
        public int WishlistId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        public Wishlist Wishlist { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
