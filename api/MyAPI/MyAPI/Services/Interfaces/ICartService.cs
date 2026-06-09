using MyAPI.Models.DTOs;

namespace MyAPI.Services.Interfaces
{
    public interface ICartService
    {
        Task<ServiceResult<CartDto>> GetCartAsync(int userId);
        Task<ServiceResult<object>> AddItemAsync(int userId, AddCartItemDto dto);
        Task<ServiceResult<object>> UpdateItemQuantityAsync(int userId, int cartItemId, UpdateCartItemDto dto);
        Task<ServiceResult<object>> DeleteItemAsync(int userId, int cartItemId);
        Task<ServiceResult<object>> ClearCartAsync(int userId);
    }
}