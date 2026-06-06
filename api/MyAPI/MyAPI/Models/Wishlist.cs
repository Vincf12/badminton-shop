using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Models
{
    [Table("wishlists")]
    public class Wishlist
    {
        [Key]
        [Column("wishlist_id")]
        public int WishlistId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        public User User { get; set; } = null!;
    }
}
