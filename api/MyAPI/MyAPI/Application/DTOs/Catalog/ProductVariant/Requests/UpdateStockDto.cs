using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Catalog.ProductVariant.Requests
{
    public class UpdateStockDto
    {
        public int StockQuantity { get; set; }
    }
}
