using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Customer.Wishlist.Requests
{
    public class AddWishlistItemDto
    {
        [Required]
        public int ProductId { get; set; }
    }
}
