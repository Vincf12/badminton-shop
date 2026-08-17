using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Order.Payment.Requests
{
    public class UpdatePaymentStatusDto
    {
        public string Status { get; set; } = string.Empty;

        public string? TransactionCode { get; set; }
    }
}
