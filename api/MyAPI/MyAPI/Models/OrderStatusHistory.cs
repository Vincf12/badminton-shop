using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Models
{
    [Table("order_status_history")]
    public class OrderStatusHistory
    {
        [Key]
        [Column("history_id")]
        public int HistoryId { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("status")]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;

        [Column("note")]
        public string? Note { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Order Order { get; set; } = null!;
    }
}