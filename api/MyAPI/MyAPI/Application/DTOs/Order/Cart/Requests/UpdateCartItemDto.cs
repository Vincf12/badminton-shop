using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Order.Cart.Requests
{
    public class UpdateCartItemDto
    {
        public int Quantity { get; set; }
    }
}
