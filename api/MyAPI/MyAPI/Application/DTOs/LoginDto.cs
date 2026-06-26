using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email không du?c d? tr?ng")]
        [EmailAddress(ErrorMessage = "Email không dúng d?nh d?ng")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "M?t kh?u không du?c d? tr?ng")]
        public string Password { get; set; } = string.Empty;
    }
}
