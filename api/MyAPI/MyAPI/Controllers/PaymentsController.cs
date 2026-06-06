using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.DTOs;
using System.Security.Claims;

namespace MyAPI.Controllers
{
    [Route("api/payments")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentsController(AppDbContext context)
        {
            _context = context;
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out userId);
        }

        private bool IsAdminOrStaff()
        {
            return User.IsInRole("admin") || User.IsInRole("staff");
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

        private async Task<Order?> GetAccessibleOrderAsync(int orderId)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return null;
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return null;
            }

            if (!IsAdminOrStaff() && order.UserId != userId)
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

        [HttpGet("order/{orderId:int}")]
        public async Task<IActionResult> GetPaymentByOrder(int orderId)
        {
            var order = await GetAccessibleOrderAsync(orderId);

            if (order == null)
            {
                return NotFound(new { message = "Không tìm thấy đơn hàng hoặc bạn không có quyền xem." });
            }

            var payment = await _context.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.OrderId == orderId);

            if (payment == null)
            {
                return NotFound(new { message = "Đơn hàng chưa có thông tin thanh toán." });
            }

            return Ok(MapPayment(payment));
        }

        [HttpPost("vnpay/create")]
        public async Task<IActionResult> CreateVnPayPayment([FromBody] CreateGatewayPaymentDto dto)
        {
            var order = await GetAccessibleOrderAsync(dto.OrderId);

            if (order == null)
            {
                return NotFound(new { message = "Không tìm thấy đơn hàng hoặc bạn không có quyền thanh toán." });
            }

            var payment = await GetOrCreatePaymentAsync(order, "vnpay");
            payment.PaymentStatus = "pending";
            payment.TransactionCode = $"VNPAY-{order.OrderCode}-{DateTime.UtcNow:yyyyMMddHHmmss}";

            await _context.SaveChangesAsync();

            var callbackUrl = $"{Request.Scheme}://{Request.Host}/api/payments/vnpay/callback";
            var paymentUrl = $"{callbackUrl}?vnp_TxnRef={order.OrderId}&vnp_ResponseCode=00&vnp_TransactionNo={Uri.EscapeDataString(payment.TransactionCode)}";

            return Ok(new
            {
                message = "Tạo URL thanh toán VNPAY thành công.",
                paymentId = payment.PaymentId,
                orderId = order.OrderId,
                amount = payment.Amount,
                paymentUrl
            });
        }

        [AllowAnonymous]
        [HttpGet("vnpay/callback")]
        public async Task<IActionResult> VnPayCallback()
        {
            var txnRef = Request.Query["vnp_TxnRef"].ToString();
            var responseCode = Request.Query["vnp_ResponseCode"].ToString();
            var transactionNo = Request.Query["vnp_TransactionNo"].ToString();

            if (!int.TryParse(txnRef, out int orderId))
            {
                return BadRequest(new { message = "Mã đơn hàng callback không hợp lệ." });
            }

            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);

            if (payment == null)
            {
                return NotFound(new { message = "Không tìm thấy thanh toán." });
            }

            payment.PaymentMethod = "vnpay";
            payment.TransactionCode = string.IsNullOrWhiteSpace(transactionNo) ? payment.TransactionCode : transactionNo;
            payment.PaymentStatus = responseCode == "00" ? "paid" : "failed";
            payment.PaidAt = responseCode == "00" ? DateTime.UtcNow : payment.PaidAt;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = responseCode == "00" ? "Thanh toán VNPAY thành công." : "Thanh toán VNPAY thất bại.",
                payment = MapPayment(payment)
            });
        }

        [HttpPost("momo/create")]
        public async Task<IActionResult> CreateMomoPayment([FromBody] CreateGatewayPaymentDto dto)
        {
            var order = await GetAccessibleOrderAsync(dto.OrderId);

            if (order == null)
            {
                return NotFound(new { message = "Không tìm thấy đơn hàng hoặc bạn không có quyền thanh toán." });
            }

            var payment = await GetOrCreatePaymentAsync(order, "momo");
            payment.PaymentStatus = "pending";
            payment.TransactionCode = $"MOMO-{order.OrderCode}-{DateTime.UtcNow:yyyyMMddHHmmss}";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Tạo thanh toán Momo thành công.",
                paymentId = payment.PaymentId,
                orderId = order.OrderId,
                amount = payment.Amount,
                payUrl = dto.ReturnUrl ?? $"{Request.Scheme}://{Request.Host}/api/payments/momo/callback",
                requestId = payment.TransactionCode
            });
        }

        [AllowAnonymous]
        [HttpPost("momo/callback")]
        public async Task<IActionResult> MomoCallback([FromBody] MomoCallbackDto dto)
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
                return NotFound(new { message = "Không tìm thấy đơn hàng." });
            }

            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == order.OrderId);

            if (payment == null)
            {
                return NotFound(new { message = "Không tìm thấy thanh toán." });
            }

            var success = dto.ResultCode == "0" || string.Equals(dto.ResultCode, "success", StringComparison.OrdinalIgnoreCase);

            payment.PaymentMethod = "momo";
            payment.TransactionCode = string.IsNullOrWhiteSpace(dto.TransactionCode) ? payment.TransactionCode : dto.TransactionCode;
            payment.PaymentStatus = success ? "paid" : "failed";
            payment.PaidAt = success ? DateTime.UtcNow : payment.PaidAt;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = success ? "Thanh toán Momo thành công." : "Thanh toán Momo thất bại.",
                payment = MapPayment(payment)
            });
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] UpdatePaymentStatusDto dto)
        {
            var validStatuses = new[] { "pending", "paid", "failed" };

            if (!validStatuses.Contains(dto.Status))
            {
                return BadRequest(new { message = "Trạng thái thanh toán không hợp lệ." });
            }

            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null)
            {
                return NotFound(new { message = "Không tìm thấy thanh toán." });
            }

            payment.PaymentStatus = dto.Status;
            payment.TransactionCode = string.IsNullOrWhiteSpace(dto.TransactionCode) ? payment.TransactionCode : dto.TransactionCode.Trim();
            payment.PaidAt = dto.Status == "paid" ? DateTime.UtcNow : payment.PaidAt;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cập nhật trạng thái thanh toán thành công.",
                payment = MapPayment(payment)
            });
        }
    }
}
