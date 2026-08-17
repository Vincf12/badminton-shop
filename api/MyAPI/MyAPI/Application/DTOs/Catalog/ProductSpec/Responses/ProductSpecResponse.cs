namespace MyAPI.Application.DTOs.Catalog.ProductSpec.Responses
{
    public class ProductSpecResponse
    {
        public int SpecId { get; set; }
        public int ProductId { get; set; }
        public string SpecName { get; set; } = string.Empty;
        public string SpecValue { get; set; } = string.Empty;
    }
}
