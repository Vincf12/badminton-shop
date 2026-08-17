using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Order.Coupon.Requests
{
    public class CouponUpsertDto
    {
        public string Code { get; set; } = string.Empty;

        public string CouponName { get; set; } = string.Empty;
        public string DiscountType { get; set; } = "percentage";

        public decimal DiscountValue { get; set; }

        public decimal MinimumOrderAmount { get; set; }

        public decimal? MaximumDiscountAmount { get; set; }

        public int? UsageLimit { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }
    }
}
