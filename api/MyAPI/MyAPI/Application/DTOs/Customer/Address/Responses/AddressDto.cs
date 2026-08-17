using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Customer.Address.Responses
{
    public class AddressDto
    {
        public int AddressId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Province { get; set; }
        public string? Ward { get; set; }
        public string? AddressDetail { get; set; }
        public bool IsDefault { get; set; }
    }
}
