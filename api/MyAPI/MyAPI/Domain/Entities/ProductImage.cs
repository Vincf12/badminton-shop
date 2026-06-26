
namespace MyAPI.Domain.Entities
{
    public class ProductImage
    {
        public int ImageId { get; set; }

        public int ProductId { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsMain { get; set; }

        public int SortOrder { get; set; }

        public Product Product { get; set; } = null!;
    }
}
