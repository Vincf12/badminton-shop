
namespace MyAPI.Domain.Entities.Store
{
    public class Banner
    {
        public long BannerId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public string TargetType { get; set; } = string.Empty;

        public string? TargetId { get; set; }

        public string? CustomUrl { get; set; }

        public string Position { get; set; } = "HOME_TOP";

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public DateTime? EndDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int Version { get; set; } = 1;
    }
}

