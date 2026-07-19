
namespace MyAPI.Domain.Entities.Order
{
    public class OrderStatusHistory
    {
        public int HistoryId { get; set; }

        public int OrderId { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Order Order { get; set; } = null!;
    }
}

