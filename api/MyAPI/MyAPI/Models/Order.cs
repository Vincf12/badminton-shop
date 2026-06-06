using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Models
{
    [Table("orders")]
    public class Order
    {
        [Key]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("coupon_id")]
        public int? CouponId { get; set; }

        [Column("order_code")]
        [StringLength(100)]
        public string OrderCode { get; set; } = string.Empty;

        [Column("shipping_recipient_name")]
        [StringLength(100)]
        public string ShippingRecipientName { get; set; } = string.Empty;

        [Column("shipping_phone")]
        [StringLength(20)]
        public string ShippingPhone { get; set; } = string.Empty;

        [Column("shipping_province")]
        [StringLength(100)]
        public string ShippingProvince { get; set; } = string.Empty;

        [Column("shipping_district")]
        [StringLength(100)]
        public string ShippingDistrict { get; set; } = string.Empty;

        [Column("shipping_ward")]
        [StringLength(100)]
        public string ShippingWard { get; set; } = string.Empty;

        [Column("shipping_address_detail")]
        public string? ShippingAddressDetail { get; set; }

        [Column("total_amount" , TypeName = "decimal(12,2)")]
        public decimal TotalAmount { get; set; } 

        [Column("shipping_fee", TypeName = "decimal(12,2)")]
        public decimal ShippingFee { get; set; }

        [Column("discount_amount", TypeName = "decimal(12,2)")]
        public decimal DiscountAmount { get; set; }

        [Column("final_amount", TypeName = "decimal(12,2)")]
        public decimal FinalAmount { get; set; }

        [Column("status")]
        [StringLength(50)]
        public string Status { get; set; } = "pending";

        [Column("tracking_code")]
        [StringLength(100)]
        public string? TrackingCode { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public User User { get; set; } = null!;
        public Coupon? Coupon { get; set; }

    }
}