using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;
    }

    public class CategoryUpsertDto
    {
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;
    }
}