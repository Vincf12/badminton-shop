using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Order.Cart.Requests
{
    public class AddCartItemDto
    {
        public int VariantId { get; set; }

        public int Quantity { get; set; } = 1;
    }
}
