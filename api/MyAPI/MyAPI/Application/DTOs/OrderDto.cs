using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateOrderDto
    {
        [Required]
        public int AddressId { get; set; }

        public string? CouponCode { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = "COD";
    }

    public class UpdateOrderStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;

        public string? Note { get; set; }
    }
}