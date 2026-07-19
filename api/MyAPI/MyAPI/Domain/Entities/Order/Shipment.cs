
namespace MyAPI.Domain.Entities.Order
{
    public class Shipment
    {
        public int ShipmentId { get; set; }

        public int OrderId { get; set; }

        public string? TrackingNumber { get; set; }

        public string? Courier { get; set; }

        public DateTime? ShippedDate { get; set; }

        public DateTime? DeliveredDate { get; set; }

        public string Status { get; set; } = "preparing";

        public Order Order { get; set; } = null!;
    }
}

