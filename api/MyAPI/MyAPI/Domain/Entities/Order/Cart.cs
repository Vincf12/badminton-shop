
namespace MyAPI.Domain.Entities.Order
{
    public class Cart
    {
        public int CartId { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; }
        public User User { get; set; } = null!;
    }
}

