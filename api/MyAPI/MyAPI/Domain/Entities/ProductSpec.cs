
namespace MyAPI.Domain.Entities
{
    public class ProductSpec
    {
        public int SpecId { get; set; }

        public int ProductId { get; set; }

        public string SpecName { get; set; } = string.Empty;

        public string SpecValue { get; set; } = string.Empty;

        public Product Product { get; set; } = null!;
    }
}
