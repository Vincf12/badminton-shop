using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Models
{
    [Table("carts")]
    public class Cart
    {
        [Key]
        [Column("cart_id")]
        public int CartId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }
        public User User { get; set; } = null!;
    }
}