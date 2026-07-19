
namespace MyAPI.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<ServiceResult<object>> RegisterAsync(RegisterDto dto);
        Task<ServiceResult<object>> LoginAsync(LoginDto dto);
    }
}


