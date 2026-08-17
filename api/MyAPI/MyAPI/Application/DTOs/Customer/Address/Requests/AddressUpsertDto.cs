using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Customer.Address.Requests
{
    public class AddressUpsertDto
    {
        public string RecipientName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string? Province { get; set; }

        public string? Ward { get; set; }

        public string? AddressDetail { get; set; }

        public bool IsDefault { get; set; } = false;
    }
}
