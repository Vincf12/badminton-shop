using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs
{
    public class CouponDto
    {
        public int CouponId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string CouponName { get; set; } = string.Empty;
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public decimal MinimumOrderAmount { get; set; }
        public decimal? MaximumDiscountAmount { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class CouponUpsertDto
    {
        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string CouponName { get; set; } = string.Empty;

        [Required]
        public string DiscountType { get; set; } = "percentage";

        public decimal DiscountValue { get; set; }

        public decimal MinimumOrderAmount { get; set; }

        public decimal? MaximumDiscountAmount { get; set; }

        public int? UsageLimit { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }
    }

    public class ApplyCouponDto
    {
        [Required]
        public string Code { get; set; } = string.Empty;

        [Range(0, 999999999)]
        public decimal OrderAmount { get; set; }
    }
}