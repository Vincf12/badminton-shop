using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Order.Order.Requests
{
    public class UpdateOrderStatusDto
    {
        public string Status { get; set; } = string.Empty;

        public string? Note { get; set; }
    }
}
