using MyAPI.Application.DTOs;

namespace MyAPI.Services.Interfaces
{
    public interface IShipmentService
    {
        Task<ServiceResult<object>> GetShipmentByOrderAsync(int orderId, int currentUserId, bool isAdminOrStaff);
        Task<ServiceResult<object>> CreateShipmentAsync(CreateShipmentDto dto);
        Task<ServiceResult<object>> UpdateShipmentAsync(int id, UpdateShipmentDto dto);
        Task<ServiceResult<object>> UpdateShipmentStatusAsync(int id, UpdateShipmentStatusDto dto);
    }
}
