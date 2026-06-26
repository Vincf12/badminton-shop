
namespace MyAPI.Domain.Entities
{
    public class Address
    {
        public int AddressId { get; set; }

        public int UserId { get; set; }

        public string RecipientName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string? Province { get; set; }

        public string? Ward { get; set; }

        public string? AddressDetail { get; set; }

        public bool IsDefault { get; set; } = false;

        public User User { get; set; } = null!;
    }
}
