using MyAPI.Application.DTOs;

namespace MyAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductListItemDto>> GetProductsAsync(string? search, int? categoryId);
        Task<ServiceResult<ProductDetailDto>> GetProductAsync(int id);
        Task<ServiceResult<ProductDetailDto>> GetProductBySlugAsync(string slug);
        Task<ServiceResult<ProductDetailDto>> CreateProductAsync(ProductUpsertDto dto);
        Task<ServiceResult<object>> UpdateProductAsync(int id, ProductUpsertDto dto);
        Task<ServiceResult<object>> DeleteProductAsync(int id);
        Task<ServiceResult<object>> SearchProductsAsync(string keyword);
        Task<ServiceResult<object>> FilterProductsAsync(int? categoryId, int? brandId, decimal? minPrice, decimal? maxPrice);
    }
}
