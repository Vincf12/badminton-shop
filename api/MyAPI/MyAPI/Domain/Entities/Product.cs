
namespace MyAPI.Domain.Entities
{
    public class Product
    {
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        public int BrandId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string? Slug { get; set; }

        public string? ShortDescription { get; set; }

        public string Status { get; set; } = "active";

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public Category Category { get; set; } = null!;
        public Brand Brand { get; set; } = null!;
    }
}
