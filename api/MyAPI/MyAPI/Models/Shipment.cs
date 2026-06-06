using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Models
{
    [Table("shipments")]
    public class Shipment
    {
        [Key]
        [Column("shipment_id")]
        public int ShipmentId { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("tracking_number")]
        [StringLength(100)]
        public string? TrackingNumber { get; set; }

        [Column("courier")]
        [StringLength(100)]
        public string? Courier { get; set; }

        [Column("shipped_date")]
        public DateTime? ShippedDate { get; set; }

        [Column("delivered_date")]
        public DateTime? DeliveredDate { get; set; }

        [Column("status")]
        [StringLength(50)]
        public string Status { get; set; } = "preparing";

        public Order Order { get; set; } = null!;
    }
}
