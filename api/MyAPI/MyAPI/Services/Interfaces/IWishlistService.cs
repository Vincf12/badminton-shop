using MyAPI.Models.DTOs;

namespace MyAPI.Services.Interfaces
{
    public interface IWishlistService
    {
        Task<ServiceResult<object>> GetWishlistAsync(int userId);
        Task<ServiceResult<object>> AddWishlistItemAsync(int userId, AddWishlistItemDto dto);
        Task<ServiceResult<object>> DeleteWishlistItemAsync(int userId, int productId);
        Task<ServiceResult<object>> CheckWishlistItemAsync(int userId, int productId);
    }
}
