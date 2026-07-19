using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Content
{
    public class BannerDto
    {
        public long BannerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string TargetType { get; set; } = string.Empty;
        public string? TargetId { get; set; }
        public string? CustomUrl { get; set; }
        public string Position { get; set; } = "HOME_TOP";
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int Version { get; set; }
    }

    public class BannerUpsertDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string TargetType { get; set; } = string.Empty;

        [StringLength(36)]
        public string? TargetId { get; set; }

        [StringLength(500)]
        public string? CustomUrl { get; set; }

        [Required]
        [StringLength(50)]
        public string Position { get; set; } = "HOME_TOP";

        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class BannerStatusUpdateDto
    {
        public bool IsActive { get; set; }
    }
}
