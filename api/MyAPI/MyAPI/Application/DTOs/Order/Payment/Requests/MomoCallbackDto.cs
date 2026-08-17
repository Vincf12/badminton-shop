using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Order.Payment.Requests
{
    public class MomoCallbackDto
    {
        public int? OrderId { get; set; }
        public string? OrderCode { get; set; }
        public string? TransactionCode { get; set; }
        public string? ResultCode { get; set; }
        public string? Message { get; set; }
    }
}
