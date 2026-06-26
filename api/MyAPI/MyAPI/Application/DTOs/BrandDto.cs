using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs
{
    public class BrandDto
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
    }

    public class BrandUpsertDto
    {
        [Required(ErrorMessage = "Tên thương hiệu không được để trống")]
        [StringLength(100)]
        public string BrandName { get; set; } = string.Empty;
    }
}
