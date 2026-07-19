using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Auth
{
    public class ChangePasswordDto
    {
        public string OldPassword { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
