
namespace MyAPI.Domain.Entities.Customer
{
    public class Wishlist
    {
        public int WishlistId { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;
    }
}

