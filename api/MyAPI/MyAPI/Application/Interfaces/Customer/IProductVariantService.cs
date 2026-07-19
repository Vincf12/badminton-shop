
namespace MyAPI.Application.Interfaces.Customer
{
    public interface IProductVariantService
    {
        Task<IEnumerable<ProductVariantDto>> GetVariantsByProductAsync(int productId);
        Task<ServiceResult<ProductVariantDto>> GetVariantAsync(int id);
        Task<ServiceResult<object>> CreateVariantAsync(int productId, ProductVariantUpsertDto dto);
        Task<ServiceResult<object>> UpdateVariantAsync(int id, ProductVariantUpsertDto dto);
        Task<ServiceResult<object>> UpdateStockAsync(int id, UpdateStockDto dto);
        Task<ServiceResult<object>> DeleteVariantAsync(int id);
    }
}


