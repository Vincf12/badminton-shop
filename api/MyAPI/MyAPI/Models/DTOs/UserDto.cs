using System.ComponentModel.DataAnnotations;

namespace MyAPI.Models.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }
    }

    public class UpdateUserRoleDto
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }

    public class UpdateUserStatusDto
    {
        public bool IsActive { get; set; }
    }
}