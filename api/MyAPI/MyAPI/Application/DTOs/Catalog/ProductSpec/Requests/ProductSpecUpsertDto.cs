using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Catalog.ProductSpec.Requests
{
    public class ProductSpecUpsertDto
    {
        public string SpecName { get; set; } = string.Empty;

        public string SpecValue { get; set; } = string.Empty;
    }
}
