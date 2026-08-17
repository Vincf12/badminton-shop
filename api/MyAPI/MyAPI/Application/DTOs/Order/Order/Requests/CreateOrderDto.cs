using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Order.Order.Requests
{
    public class CreateOrderDto
    {
        public int AddressId { get; set; }

        public string? CouponCode { get; set; }

        public string PaymentMethod { get; set; } = "COD";
    }
}
