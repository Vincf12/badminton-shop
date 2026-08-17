
namespace MyAPI.Application.Interfaces.Customer
{
    public interface IProductSpecService
    {
        Task<IEnumerable<ProductSpecResponse>> GetSpecsAsync(int productId);
        Task<ServiceResult<object>> CreateSpecAsync(int productId, ProductSpecUpsertDto dto);
        Task<ServiceResult<object>> UpdateSpecAsync(int id, ProductSpecUpsertDto dto);
        Task<ServiceResult<object>> DeleteSpecAsync(int id);
    }
}


