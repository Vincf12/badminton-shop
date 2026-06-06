using System.ComponentModel.DataAnnotations;

namespace MyAPI.Models.DTOs
{
    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateReviewDto
    {
        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }
    }

    public class UpdateReviewDto
    {
        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }
    }
}
