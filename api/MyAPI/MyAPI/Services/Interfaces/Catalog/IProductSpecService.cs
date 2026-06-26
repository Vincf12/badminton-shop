using MyAPI.Application.DTOs;

namespace MyAPI.Services.Interfaces
{
    public interface IProductSpecService
    {
        Task<IEnumerable<ProductSpecDto>> GetSpecsAsync(int productId);
        Task<ServiceResult<object>> CreateSpecAsync(int productId, ProductSpecUpsertDto dto);
        Task<ServiceResult<object>> UpdateSpecAsync(int id, ProductSpecUpsertDto dto);
        Task<ServiceResult<object>> DeleteSpecAsync(int id);
    }
}
