
namespace MyAPI.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }


        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Gender { get; set; } = "other";

        public DateTime? Birthdate { get; set; }

        public string? Role { get; set; } = "customer";

        public bool IsActive { get; set; } = true;

        public bool EmailVerified { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
