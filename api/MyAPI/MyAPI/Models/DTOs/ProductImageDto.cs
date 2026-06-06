using System.ComponentModel.DataAnnotations;

namespace MyAPI.Models.DTOs
{
    public class ProductImageDto
    {
        public int ImageId { get; set; }
        public int ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public int SortOrder { get; set; }
    }

    public class ProductImageUpsertDto
    {
        [Required]
        [StringLength(255)]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsMain { get; set; }

        public int SortOrder { get; set; }
    }
}