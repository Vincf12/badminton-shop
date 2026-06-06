using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Models
{
    [Table("coupons")]
    public class Coupon
    {
        [Key]
        [Column("coupon_id")]
        public int CouponId { get; set; }

        [Required]
        [Column("code")]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Column("coupon_name")]
        [StringLength(200)]
        public string CouponName { get; set; } = string.Empty;

        [Column("discount_type")]
        [StringLength(20)]
        public string DiscountType { get; set; } = "percentage";

        [Column("discount_value")]
        public decimal DiscountValue { get; set; }

        [Column("minimum_order_amount")]
        public decimal MinimumOrderAmount { get; set; }

        [Column("maximum_discount_amount")]
        public decimal? MaximumDiscountAmount { get; set; }

        [Column("usage_limit")]
        public int? UsageLimit { get; set; }

        [Column("used_count")]
        public int UsedCount { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;
    }
}