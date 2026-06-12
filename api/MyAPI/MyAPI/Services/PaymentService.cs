using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.DTOs;
using MyAPI.Services.Interfaces;

namespace MyAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        private static PaymentDto MapPayment(Payment payment)
        {
            return new PaymentDto
            {
                PaymentId = payment.PaymentId,
                OrderId = payment.OrderId,
                PaymentMethod = payment.PaymentMethod,
                PaymentStatus = payment.PaymentStatus,
                Amount = payment.Amount,
                TransactionCode = payment.TransactionCode,
                PaidAt = payment.PaidAt,
                CreatedAt = payment.CreatedAt
            };
        }

        private async Task<Order?> GetAccessibleOrderAsync(int orderId, int currentUserId, bool isAdminOrStaff)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return null;
            }

            if (!isAdminOrStaff && order.UserId != currentUserId)
            {
                return null;
            }

            return order;
        }

        private async Task<Payment> GetOrCreatePaymentAsync(Order order, string method)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == order.OrderId);

            if (payment != null)
            {
                payment.PaymentMethod = method;
                payment.Amount = order.FinalAmount;
                return payment;
            }

            payment = new Payment
            {
                OrderId = order.OrderId,
                PaymentMethod = method,
                PaymentStatus = "pending",
                Amount = order.FinalAmount,
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            return payment;
        }

        public async Task<ServiceResult<object>> GetPaymentByOrderAsync(int orderId, int currentUserId, bool isAdminOrStaff)
        {
            var order = await GetAccessibleOrderAsync(orderId, currentUserId, isAdminOrStaff);

            if (order == null)
            {
                return ServiceResult<object>.NotFound("Không tìm thấy đơn hàng hoặc bạn không có quyền xem.");
            }

            var payment = await _context.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.OrderId == orderId);

            if (payment == null)
            {
                return ServiceResult<object>.NotFound("Đơn hàng chưa có thông tin thanh toán.");
            }

            return ServiceResult<object>.Ok(MapPayment(payment));
        }

        public async Task<ServiceResult<object>> CreateVnPayPaymentAsync(CreateGatewayPaymentDto dto, int currentUserId, bool isAdminOrStaff, string callbackUrl)
        {
            var order = await GetAccessibleOrderAsync(dto.OrderId, currentUserId, isAdminOrStaff);

            if (order == null)
            {
                return ServiceResult<object>.NotFound("Không tìm thấy đơn hàng hoặc bạn không có quyền thanh toán.");
            }

            var payment = await GetOrCreatePaymentAsync(order, "vnpay");
            payment.PaymentStatus = "pending";
            payment.TransactionCode = $"VNPAY-{order.OrderCode}-{DateTime.UtcNow:yyyyMMddHHmmss}";

            await _context.SaveChangesAsync();

            var paymentUrl = $"{callbackUrl}?vnp_TxnRef={order.OrderId}&vnp_ResponseCode=00&vnp_TransactionNo={Uri.EscapeDataString(payment.TransactionCode)}";

            return ServiceResult<object>.Ok(new
            {
                message = "Tạo URL thanh toán VNPAY thành công.",
                paymentId = payment.PaymentId,
                orderId = order.OrderId,
                amount = payment.Amount,
                paymentUrl
            });
        }

        public async Task<ServiceResult<object>> VnPayCallbackAsync(string txnRef, string responseCode, string transactionNo)
        {
            if (!int.TryParse(txnRef, out int orderId))
            {
                return ServiceResult<object>.BadRequest("Mã đơn hàng callback không hợp lệ.");
            }

            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);

            if (payment == null)
            {
                return ServiceResult<object>.NotFound("Không tìm thấy thanh toán.");
            }

            payment.PaymentMethod = "vnpay";
            payment.TransactionCode = string.IsNullOrWhiteSpace(transactionNo) ? payment.TransactionCode : transactionNo;
            payment.PaymentStatus = responseCode == "00" ? "paid" : "failed";
            payment.PaidAt = responseCode == "00" ? DateTime.UtcNow : payment.PaidAt;

            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new
            {
                message = responseCode == "00" ? "Thanh toán VNPAY thành công." : "Thanh toán VNPAY thất bại.",
                payment = MapPayment(payment)
            });
        }

        public async Task<ServiceResult<object>> CreateMomoPaymentAsync(CreateGatewayPaymentDto dto, int currentUserId, bool isAdminOrStaff, string fallbackPayUrl)
        {
            var order = await GetAccessibleOrderAsync(dto.OrderId, currentUserId, isAdminOrStaff);

            if (order == null)
            {
                return ServiceResult<object>.NotFound("Không tìm thấy đơn hàng hoặc bạn không có quyền thanh toán.");
            }

            var payment = await GetOrCreatePaymentAsync(order, "momo");
            payment.PaymentStatus = "pending";
            payment.TransactionCode = $"MOMO-{order.OrderCode}-{DateTime.UtcNow:yyyyMMddHHmmss}";

            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new
            {
                message = "Tạo thanh toán Momo thành công.",
                paymentId = payment.PaymentId,
                orderId = order.OrderId,
                amount = payment.Amount,
                payUrl = dto.ReturnUrl ?? fallbackPayUrl,
                requestId = payment.TransactionCode
            });
        }

        public async Task<ServiceResult<object>> MomoCallbackAsync(MomoCallbackDto dto)
        {
            Order? order = null;

            if (dto.OrderId.HasValue)
            {
                order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == dto.OrderId.Value);
            }
            else if (!string.IsNullOrWhiteSpace(dto.OrderCode))
            {
                order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderCode == dto.OrderCode);
            }

            if (order == null)
            {
                return ServiceResult<object>.NotFound("Không tìm thấy đơn hàng.");
            }

            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == order.OrderId);

            if (payment == null)
            {
                return ServiceResult<object>.NotFound("Không tìm thấy thanh toán.");
            }

            var success = dto.ResultCode == "0" || string.Equals(dto.ResultCode, "success", StringComparison.OrdinalIgnoreCase);

            payment.PaymentMethod = "momo";
            payment.TransactionCode = string.IsNullOrWhiteSpace(dto.TransactionCode) ? payment.TransactionCode : dto.TransactionCode;
            payment.PaymentStatus = success ? "paid" : "failed";
            payment.PaidAt = success ? DateTime.UtcNow : payment.PaidAt;

            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new
            {
                message = success ? "Thanh toán Momo thành công." : "Thanh toán Momo thất bại.",
                payment = MapPayment(payment)
            });
        }

        public async Task<ServiceResult<object>> UpdatePaymentStatusAsync(int id, UpdatePaymentStatusDto dto)
        {
            var validStatuses = new[] { "pending", "paid", "failed" };

            if (!validStatuses.Contains(dto.Status))
            {
                return ServiceResult<object>.BadRequest("Trạng thái thanh toán không hợp lệ.");
            }

            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null)
            {
                return ServiceResult<object>.NotFound("Không tìm thấy thanh toán.");
            }

            payment.PaymentStatus = dto.Status;
            payment.TransactionCode = string.IsNullOrWhiteSpace(dto.TransactionCode) ? payment.TransactionCode : dto.TransactionCode.Trim();
            payment.PaidAt = dto.Status == "paid" ? DateTime.UtcNow : payment.PaidAt;

            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new
            {
                message = "Cập nhật trạng thái thanh toán thành công.",
                payment = MapPayment(payment)
            });
        }
    }
}
