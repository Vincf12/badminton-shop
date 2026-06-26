
namespace MyAPI.Domain.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        public string PaymentMethod { get; set; } = "cod";

        public string PaymentStatus { get; set; } = "pending";

        public decimal Amount { get; set; }

        public string? TransactionCode { get; set; }

        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Order Order { get; set; } = null!;
    }
}
