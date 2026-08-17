using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Order.Coupon.Requests
{
    public class ApplyCouponDto
    {
        public string Code { get; set; } = string.Empty;

        public decimal OrderAmount { get; set; }
    }
}
