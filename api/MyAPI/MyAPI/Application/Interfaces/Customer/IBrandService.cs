
namespace MyAPI.Application.Interfaces.Customer
{
    public interface IBrandService
    {
        Task<IEnumerable<BrandDto>> GetBrandsAsync();
        Task<ServiceResult<BrandDto>> GetBrandAsync(int id);
        Task<ServiceResult<BrandDto>> CreateBrandAsync(BrandUpsertDto dto);
        Task<ServiceResult<object>> UpdateBrandAsync(int id, BrandUpsertDto dto);
        Task<ServiceResult<object>> DeleteBrandAsync(int id);
    }
}


