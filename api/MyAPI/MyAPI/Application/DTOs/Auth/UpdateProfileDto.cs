using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Auth
{
    public class UpdateProfileDto
    {
        public string FullName { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public DateOnly? Birthdate { get; set; }
    }
}
