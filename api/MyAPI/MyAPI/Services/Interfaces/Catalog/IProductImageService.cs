using MyAPI.Application.DTOs;

namespace MyAPI.Services.Interfaces
{
    public interface IProductImageService
    {
        Task<IEnumerable<ProductImageDto>> GetImagesAsync(int productId);
        Task<ServiceResult<object>> CreateImageAsync(int productId, ProductImageUpsertDto dto);
        Task<ServiceResult<object>> UpdateImageAsync(int id, ProductImageUpsertDto dto);
        Task<ServiceResult<object>> SetMainImageAsync(int id);
        Task<ServiceResult<object>> DeleteImageAsync(int id);
    }
}
