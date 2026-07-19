namespace MyAPI.Application.DTOs.Catalog.ProductSpec
{
    public class ProductSpecDto
    {
        public int SpecId { get; set; }
        public int ProductId { get; set; }
        public string SpecName { get; set; } = string.Empty;
        public string SpecValue { get; set; } = string.Empty;
    }
}
