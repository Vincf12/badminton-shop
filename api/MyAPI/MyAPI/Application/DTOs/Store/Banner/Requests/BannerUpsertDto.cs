using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Store.Banner.Requests
{
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
}
