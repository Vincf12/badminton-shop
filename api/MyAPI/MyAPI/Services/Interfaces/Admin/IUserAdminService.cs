using MyAPI.Application.DTOs;

namespace MyAPI.Services.Interfaces
{
    public interface IUserAdminService
    {
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<ServiceResult<UserDto>> GetUserByIdAsync(int id);
        Task<ServiceResult<object>> UpdateUserRoleAsync(int id, UpdateUserRoleDto dto);
        Task<ServiceResult<object>> UpdateUserStatusAsync(int id, UpdateUserStatusDto dto);
        Task<ServiceResult<object>> DeleteUserAsync(int id);
    }
}
