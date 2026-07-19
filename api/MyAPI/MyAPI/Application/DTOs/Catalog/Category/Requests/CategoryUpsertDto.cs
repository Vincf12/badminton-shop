using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Catalog.Category.Requests
{
    public class CategoryUpsertDto
    {
        public string CategoryName { get; set; } = string.Empty;
    }
}
