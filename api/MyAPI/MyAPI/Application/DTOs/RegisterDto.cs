using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs
{
    public class RegisterDto
    {

        [Required(ErrorMessage = "Email không du?c d? tr?ng")]
        [EmailAddress(ErrorMessage = "Email không dúng d?nh d?ng")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "M?t kh?u không du?c d? tr?ng")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "M?t kh?u ph?i t? 6 ký t? tr? lên")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "H? và tên không du?c d? tr?ng")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

            [StringLength(20)]
            public string? Phone { get; set; }
    }
}
