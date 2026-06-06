using System.ComponentModel.DataAnnotations;

namespace MyAPI.Models.DTOs
{
    public class ProductSpecDto
    {
        public int SpecId { get; set; }
        public int ProductId { get; set; }
        public string SpecName { get; set; } = string.Empty;
        public string SpecValue { get; set; } = string.Empty;
    }

    public class ProductSpecUpsertDto
    {
        [Required]
        [StringLength(100)]
        public string SpecName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string SpecValue { get; set; } = string.Empty;
    }
}