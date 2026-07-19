namespace MyAPI.Application.DTOs.Catalog.Product.Responses
{
    public class ProductDetailDto : ProductListItemDto
    {
        public DateTime? UpdatedAt { get; set; }
    }
}
