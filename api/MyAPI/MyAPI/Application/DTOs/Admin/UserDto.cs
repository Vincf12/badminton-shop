namespace MyAPI.Application.DTOs.Admin
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

    public class UpdateUserRoleDto
    {
        public string Role { get; set; } = string.Empty;
    }

    public class UpdateUserStatusDto
    {
        public bool IsActive { get; set; }
    }
}
