using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime? Birthdate { get; set; }
        public string? Role { get; set; }
    }

    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "Ho ten khong duoc de trong")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        public DateTime? Birthdate { get; set; }
    }
}
