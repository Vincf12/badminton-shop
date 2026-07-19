using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Catalog.Product.Responses
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
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsMain { get; set; }

        public int SortOrder { get; set; }
    }
}
