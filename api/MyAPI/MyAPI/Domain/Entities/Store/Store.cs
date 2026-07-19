namespace MyAPI.Domain.Entities.Store
{
    public class Store
    {
        public int StoreId { get; set; }
        public string StoreCode { get; set; } = string.Empty;

        public string StoreName { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Province { get; set; } = null!;

        public string Ward { get; set; } = null!;
        public string AddressDetail { get; set; } = null!;

        public bool IsActive { get; set; } = true;
        public bool IsPickupPoint { get; set; }

        public TimeOnly? OpeningTime { get; set; }

        public TimeOnly? ClosingTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
