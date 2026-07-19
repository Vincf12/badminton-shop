
namespace MyAPI.Application.Interfaces.Order
{
    public interface IPaymentService
    {
        Task<ServiceResult<object>> GetPaymentByOrderAsync(int orderId, int currentUserId, bool isAdminOrStaff);
        Task<ServiceResult<object>> CreateCodPaymentAsync(CreateGatewayPaymentDto dto, int currentUserId, bool isAdminOrStaff);
        Task<ServiceResult<object>> CreateVnPayPaymentAsync(CreateGatewayPaymentDto dto, int currentUserId, bool isAdminOrStaff, string callbackUrl);
        Task<ServiceResult<object>> VnPayCallbackAsync(string txnRef, string responseCode, string transactionNo);
        Task<ServiceResult<object>> CreateMomoPaymentAsync(CreateGatewayPaymentDto dto, int currentUserId, bool isAdminOrStaff, string fallbackPayUrl);
        Task<ServiceResult<object>> MomoCallbackAsync(MomoCallbackDto dto);
        Task<ServiceResult<object>> UpdatePaymentStatusAsync(int id, UpdatePaymentStatusDto dto);
    }
}


