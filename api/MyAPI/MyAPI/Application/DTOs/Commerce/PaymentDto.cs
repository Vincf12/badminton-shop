using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Commerce
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? TransactionCode { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateGatewayPaymentDto
    {
        public int OrderId { get; set; }

        public string? ReturnUrl { get; set; }
    }

    public class UpdatePaymentStatusDto
    {
        public string Status { get; set; } = string.Empty;

        public string? TransactionCode { get; set; }
    }

    public class MomoCallbackDto
    {
        public int? OrderId { get; set; }
        public string? OrderCode { get; set; }
        public string? TransactionCode { get; set; }
        public string? ResultCode { get; set; }
        public string? Message { get; set; }
    }
}
