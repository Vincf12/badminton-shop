using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("username")]
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("password_hash")]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [Column("full_name")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Column("phone")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [Column("avatar_url")]
        [StringLength(255)]
        public string? AvatarUrl { get; set; }

        [Column("gender")]
        public string? Gender { get; set; } = "other";

        [Column("birthdate")]
        public DateTime? Birthday { get; set; }

        [Column("role")]
        public string? Role { get; set; } = "customer";

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("email_verified")]
        public bool EmailVerified { get; set; } = false;

        [Column("last_login")]
        public DateTime? LastLogin { get; set; }
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}