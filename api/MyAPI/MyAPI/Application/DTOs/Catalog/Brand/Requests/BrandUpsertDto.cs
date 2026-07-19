using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Catalog.Brand.Requests
{
    public class BrandUpsertDto
    {
        public string BrandName { get; set; } = string.Empty;
    }
}
