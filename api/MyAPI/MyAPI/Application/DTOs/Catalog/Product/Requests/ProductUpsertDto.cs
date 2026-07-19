using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Catalog.Product.Requests
{
    public class ProductUpsertDto
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        public string? Slug { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "active";

        public string? ImageUrl { get; set; }

        [Range(typeof(decimal), "0", "999999999999")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }
}
