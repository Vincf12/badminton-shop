using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs
{
    public class ShipmentDto
    {
        public int ShipmentId { get; set; }
        public int OrderId { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Courier { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class CreateShipmentDto
    {
        [Required]
        public int OrderId { get; set; }

        [StringLength(100)]
        public string? TrackingNumber { get; set; }

        [StringLength(100)]
        public string? Courier { get; set; }

        public DateTime? ShippedDate { get; set; }

        public DateTime? DeliveredDate { get; set; }

        public string Status { get; set; } = "preparing";
    }

    public class UpdateShipmentDto
    {
        [StringLength(100)]
        public string? TrackingNumber { get; set; }

        [StringLength(100)]
        public string? Courier { get; set; }

        public DateTime? ShippedDate { get; set; }

        public DateTime? DeliveredDate { get; set; }

        public string Status { get; set; } = "preparing";
    }

    public class UpdateShipmentStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
