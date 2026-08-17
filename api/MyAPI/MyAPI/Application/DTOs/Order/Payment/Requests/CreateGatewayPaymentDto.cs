using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Order.Payment.Requests
{
    public class CreateGatewayPaymentDto
    {
        public int OrderId { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
