
namespace MyAPI.Application.Interfaces.Order
{
    public interface IOrderService
    {
        Task<ServiceResult<object>> CreateOrderAsync(int userId, CreateOrderDto dto);
        Task<ServiceResult<object>> GetMyOrdersAsync(int userId);
        Task<ServiceResult<object>> GetAllOrdersAsync();
        Task<ServiceResult<object>> GetOrderDetailAsync(int orderId, int currentUserId, bool isAdminOrStaff);
        Task<ServiceResult<object>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto);
        Task<ServiceResult<object>> CancelOrderAsync(int orderId, int currentUserId, bool isAdminOrStaff);
    }
}


