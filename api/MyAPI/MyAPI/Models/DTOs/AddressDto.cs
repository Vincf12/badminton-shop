using System.ComponentModel.DataAnnotations;

namespace MyAPI.Models.DTOs
{
    public class AddressDto
    {
        public int AddressId { get; set; }
        public string RecipientName  { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Province { get; set; }
        public string? Ward { get; set; }
        public string? District { get; set; }
        public string? AddressDetail { get; set; }
        public bool IsDefault { get; set; }
    }

    public class AddressUpsertDto
    {
        [Required(ErrorMessage = "Tên người nhận không được để trống")]
        public string RecipientName  { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string ? Province { get; set; }

        public string? Ward { get; set; }

        public string? District { get; set; }

        public string? AddressDetail { get; set; }

        public bool IsDefault { get; set; } = false;
    }
}
