using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Order.Shipment.Requests
{
    public class UpdateShipmentStatusDto
    {
        public string Status { get; set; } = string.Empty;
    }
}
