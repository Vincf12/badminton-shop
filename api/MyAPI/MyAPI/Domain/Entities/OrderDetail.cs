
namespace MyAPI.Domain.Entities
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }

        public int OrderId { get; set; }

        public int VariantId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal SubTotal { get; set; }

        public Order Order { get; set; } = null!;
        public ProductVariant Variant { get; set; } = null!;
    }
}
