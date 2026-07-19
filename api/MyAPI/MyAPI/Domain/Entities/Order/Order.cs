
namespace MyAPI.Domain.Entities.Order
{
    public class Order
    {
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public int? CouponId { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public string ShippingRecipientName { get; set; } = string.Empty;

        public string ShippingPhone { get; set; } = string.Empty;

        public string ShippingProvince { get; set; } = string.Empty;

        public string ShippingDistrict { get; set; } = string.Empty;

        public string ShippingWard { get; set; } = string.Empty;

        public string? ShippingAddressDetail { get; set; }

        public decimal TotalAmount { get; set; } 

        public decimal ShippingFee { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal FinalAmount { get; set; }

        public string Status { get; set; } = "pending";

        public string? TrackingCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public User User { get; set; } = null!;
        public Coupon? Coupon { get; set; }

    }
}

