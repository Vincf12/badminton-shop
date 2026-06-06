using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Models
{
    [Table("Addresses")]
    public class Address
    {
        [Key]
        [Column("address_id")]
        public int AddressId { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("recipient_name")]
        [StringLength(100)]
        public string RecipientName { get; set; } = string.Empty;

        [Required]
        [Column("phone")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Column("province")]
        [StringLength(100)]
        public string? Province { get; set; }

        [Column("ward")]
        [StringLength(100)]
        public string? Ward { get; set; }

        [Column("district")]
        [StringLength(100)]
        public string? District { get; set; }

        [Column("address_detail")]
        public string? AddressDetail { get; set; }

        [Column("is_default")]
        public bool IsDefault { get; set; } = false;

        public User User { get; set; } = null!;
    }
}
