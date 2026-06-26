using MyAPI.Application.DTOs;

namespace MyAPI.Services.Interfaces
{
    public interface IAccountService
    {
        Task<ServiceResult<AccountDto>> GetCurrentUserAsync(int userId);
        Task<ServiceResult<object>> UpdateProfileAsync(int userId, UpdateProfileDto dto);
        Task<ServiceResult<object>> ChangePasswordAsync(int userId, ChangePasswordDto dto);
    }
}
