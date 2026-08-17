using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Order.Shipment.Requests
{
    public class CreateShipmentDto
    {
        public int OrderId { get; set; }

        public string? TrackingNumber { get; set; }

        public string? Courier { get; set; }

        public DateTime? ShippedDate { get; set; }

        public DateTime? DeliveredDate { get; set; }

        public string Status { get; set; } = "preparing";
    }
}
