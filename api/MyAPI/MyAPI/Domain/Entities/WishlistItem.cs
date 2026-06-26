
namespace MyAPI.Domain.Entities
{
    public class WishlistItem
    {
        public int WishlistItemId { get; set; }

        public int WishlistId { get; set; }

        public int ProductId { get; set; }

        public Wishlist Wishlist { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
