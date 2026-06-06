using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Models
{
    [Table("payments")]
    public class Payment
    {
        [Key]
        [Column("payment_id")]
        public int PaymentId { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("payment_method")]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "cod";

        [Column("payment_status")]
        [StringLength(50)]
        public string PaymentStatus { get; set; } = "pending";

        [Column("amount", TypeName = "decimal(12,2)")]
        public decimal Amount { get; set; }

        [Column("transaction_code")]
        [StringLength(100)]
        public string? TransactionCode { get; set; }

        [Column("paid_at")]
        public DateTime? PaidAt { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Order Order { get; set; } = null!;
    }
}
