using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Order.Shipment.Requests
{
    public class UpdateShipmentDto
    {
        public string? TrackingNumber { get; set; }

        public string? Courier { get; set; }

        public DateTime? ShippedDate { get; set; }

        public DateTime? DeliveredDate { get; set; }

        public string Status { get; set; } = "preparing";
    }
}
