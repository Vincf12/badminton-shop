using MyAPI.Application.DTOs;

namespace MyAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<object>> RegisterAsync(RegisterDto dto);
        Task<ServiceResult<object>> LoginAsync(LoginDto dto);
    }
}
