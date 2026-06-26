using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs
{
    public class AddressDto
    {
        public int AddressId { get; set; }
        public string RecipientName  { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Province { get; set; }
        public string? Ward { get; set; }
        public string? AddressDetail { get; set; }
        public bool IsDefault { get; set; }
    }

    public class AddressUpsertDto
    {
        [Required(ErrorMessage = "Tên ngu?i nh?n không du?c d? tr?ng")]
        public string RecipientName  { get; set; } = string.Empty;

        [Required(ErrorMessage = "S? di?n tho?i không du?c d? tr?ng")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ð?a ch? không du?c d? tr?ng")]
        public string ? Province { get; set; }

        public string? Ward { get; set; }

        public string? AddressDetail { get; set; }

        public bool IsDefault { get; set; } = false;
    }
}
