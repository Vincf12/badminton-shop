using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Catalog.Review.Requests
{
    public class CreateReviewDto
    {
        public int Rating { get; set; }

        public string? Comment { get; set; }
    }
}
