using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Auth
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateOnly? Birthdate { get; set; }
        public string? Role { get; set; }
    }
}
