using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.DTOs;
using System.Security.Claims;

namespace MyAPI.Controllers
{
    [Route("api/orders")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out userId);
        }

        private static string GenerateOrderCode()
        {
            return $"ORD{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            if (!TryGetCurrentUserId(out int userId))
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.AddressId == dto.AddressId && a.UserId == userId);

                if (address == null)
                    return BadRequest(new { message = "Địa chỉ giao hàng không hợp lệ." });

                var cart = await _context.Carts
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                    return BadRequest(new { message = "Giỏ hàng trống." });

                var cartItems = await _context.CartItems
                    .Where(ci => ci.CartId == cart.CartId)
                    .ToListAsync();

                if (!cartItems.Any())
                    return BadRequest(new { message = "Giỏ hàng trống." });

                decimal totalAmount = 0;
                decimal shippingFee = 0;

                var orderDetails = new List<OrderDetail>();

                foreach (var item in cartItems)
                {
                    if (item.Quantity <= 0)
                        return BadRequest(new { message = "Số lượng sản phẩm không hợp lệ." });

                    var variant = await _context.ProductVariants
                        .FirstOrDefaultAsync(v => v.VariantId == item.VariantId);

                    if (variant == null)
                        return BadRequest(new { message = "Biến thể sản phẩm không tồn tại." });

                    if (variant.StockQuantity < item.Quantity)
                    {
                        return BadRequest(new
                        {
                            message = $"Sản phẩm {variant.Sku} không đủ tồn kho."
                        });
                    }

                    decimal subTotal = variant.Price * item.Quantity;
                    totalAmount += subTotal;

                    orderDetails.Add(new OrderDetail
                    {
                        VariantId = variant.VariantId,
                        Quantity = item.Quantity,
                        UnitPrice = variant.Price,
                        SubTotal = subTotal
                    });

                    variant.StockQuantity -= item.Quantity;
                }

                decimal discountAmount = 0;
                int? couponId = null;

                if (!string.IsNullOrWhiteSpace(dto.CouponCode))
                {
                    var code = dto.CouponCode.Trim().ToUpper();

                    var coupon = await _context.Coupons
                        .FirstOrDefaultAsync(c => c.Code == code);

                    if (coupon == null)
                        return BadRequest(new { message = "Mã giảm giá không tồn tại." });

                    if (!coupon.IsActive)
                        return BadRequest(new { message = "Mã giảm giá đã bị khóa." });

                    var now = DateTime.UtcNow;

                    if (now < coupon.StartDate || now > coupon.EndDate)
                        return BadRequest(new { message = "Mã giảm giá không còn hiệu lực." });

                    if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value)
                        return BadRequest(new { message = "Mã giảm giá đã hết lượt sử dụng." });

                    if (totalAmount < coupon.MinimumOrderAmount)
                    {
                        return BadRequest(new
                        {
                            message = $"Đơn hàng phải từ {coupon.MinimumOrderAmount:N0} VNĐ để dùng mã này."
                        });
                    }

                    if (coupon.DiscountType == "percentage")
                    {
                        discountAmount = totalAmount * coupon.DiscountValue / 100;

                        if (coupon.MaximumDiscountAmount.HasValue)
                        {
                            discountAmount = Math.Min(
                                discountAmount,
                                coupon.MaximumDiscountAmount.Value
                            );
                        }
                    }
                    else if (coupon.DiscountType == "fixed")
                    {
                        discountAmount = coupon.DiscountValue;
                    }
                    else
                    {
                        return BadRequest(new { message = "Loại mã giảm giá không hợp lệ." });
                    }

                    if (discountAmount > totalAmount)
                        discountAmount = totalAmount;

                    coupon.UsedCount++;
                    couponId = coupon.CouponId;
                }

                decimal finalAmount = totalAmount + shippingFee - discountAmount;

                var order = new Order
                {
                    UserId = userId,
                    CouponId = couponId,

                    OrderCode = GenerateOrderCode(),

                    ShippingRecipientName = address.RecipientName,
                    ShippingPhone = address.Phone,
                    ShippingProvince = address.Province ?? "",
                    ShippingDistrict = address.District ?? "",
                    ShippingWard = address.Ward ?? "",
                    ShippingAddressDetail = address.AddressDetail,

                    TotalAmount = totalAmount,
                    ShippingFee = shippingFee,
                    DiscountAmount = discountAmount,
                    FinalAmount = finalAmount,

                    Status = "pending",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var detail in orderDetails)
                {
                    detail.OrderId = order.OrderId;
                }

                _context.OrderDetails.AddRange(orderDetails);

                var paymentMethod = dto.PaymentMethod.Trim().ToLower();
                var validPaymentMethods = new[] { "cod", "vnpay", "momo" };

                if (!validPaymentMethods.Contains(paymentMethod))
                {
                    return BadRequest(new { message = "Phương thức thanh toán không hợp lệ." });
                }

                var payment = new Payment
                {
                    OrderId = order.OrderId,
                    PaymentMethod = paymentMethod,
                    PaymentStatus = "pending",
                    Amount = finalAmount,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Payments.Add(payment);

                var history = new OrderStatusHistory
                {
                    OrderId = order.OrderId,
                    Status = "pending",
                    Note = "Đơn hàng được tạo.",
                    CreatedAt = DateTime.UtcNow
                };

                _context.OrderStatusHistories.Add(history);

                _context.CartItems.RemoveRange(cartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Tạo đơn hàng thành công.",
                    orderId = order.OrderId,
                    orderCode = order.OrderCode,
                    totalAmount,
                    shippingFee,
                    discountAmount,
                    finalAmount
                });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyOrders()
        {
            if (!TryGetCurrentUserId(out int userId))
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });

            var orders = await _context.Orders
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    OrderCode = o.OrderCode,
                    UserId = o.UserId,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    ShippingFee = o.ShippingFee,
                    DiscountAmount = o.DiscountAmount,
                    FinalAmount = o.FinalAmount,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();

            return Ok(orders);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    OrderCode = o.OrderCode,
                    UserId = o.UserId,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    ShippingFee = o.ShippingFee,
                    DiscountAmount = o.DiscountAmount,
                    FinalAmount = o.FinalAmount,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrderDetail(int id)
        {
            if (!TryGetCurrentUserId(out int userId))
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });

            bool isAdminOrStaff = User.IsInRole("admin") || User.IsInRole("staff");

            var order = await _context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound(new { message = "Không tìm thấy đơn hàng." });

            if (!isAdminOrStaff && order.UserId != userId)
                return Forbid();

            var details = await (
                from d in _context.OrderDetails.AsNoTracking()
                join v in _context.ProductVariants.AsNoTracking()
                    on d.VariantId equals v.VariantId
                join p in _context.Products.AsNoTracking()
                    on v.ProductId equals p.ProductId
                where d.OrderId == id
                select new
                {
                    d.OrderDetailId,
                    d.VariantId,
                    p.ProductName,
                    v.Sku,
                    v.Weight,
                    v.GripSize,
                    v.Color,
                    d.Quantity,
                    d.UnitPrice,
                    d.SubTotal
                })
                .ToListAsync();

            return Ok(new
            {
                order.OrderId,
                order.OrderCode,
                order.UserId,

                order.ShippingRecipientName,
                order.ShippingPhone,
                order.ShippingProvince,
                order.ShippingDistrict,
                order.ShippingWard,
                order.ShippingAddressDetail,

                order.Status,
                order.TotalAmount,
                order.ShippingFee,
                order.DiscountAmount,
                order.FinalAmount,
                order.CreatedAt,
                order.UpdatedAt,

                items = details
            });
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound(new { message = "Không tìm thấy đơn hàng." });

            var validStatuses = new[] { "pending", "confirmed", "shipping", "completed", "cancelled" };

            if (!validStatuses.Contains(dto.Status))
                return BadRequest(new { message = "Trạng thái đơn hàng không hợp lệ." });

            order.Status = dto.Status;
            order.UpdatedAt = DateTime.UtcNow;

            _context.OrderStatusHistories.Add(new OrderStatusHistory
            {
                OrderId = order.OrderId,
                Status = dto.Status,
                Note = dto.Note,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật trạng thái đơn hàng thành công." });
        }

        [HttpPut("{id:int}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            if (!TryGetCurrentUserId(out int userId))
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });

            bool isAdminOrStaff = User.IsInRole("admin") || User.IsInRole("staff");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);

                if (order == null)
                    return NotFound(new { message = "Không tìm thấy đơn hàng." });

                if (!isAdminOrStaff && order.UserId != userId)
                    return Forbid();

                if (order.Status == "completed")
                    return BadRequest(new { message = "Không thể hủy đơn hàng đã hoàn thành." });

                if (order.Status == "cancelled")
                    return BadRequest(new { message = "Đơn hàng đã được hủy trước đó." });

                var details = await _context.OrderDetails
                    .Where(d => d.OrderId == order.OrderId)
                    .ToListAsync();

                foreach (var detail in details)
                {
                    var variant = await _context.ProductVariants
                        .FirstOrDefaultAsync(v => v.VariantId == detail.VariantId);

                    if (variant != null)
                    {
                        variant.StockQuantity += detail.Quantity;
                    }
                }

                order.Status = "cancelled";
                order.UpdatedAt = DateTime.UtcNow;

                _context.OrderStatusHistories.Add(new OrderStatusHistory
                {
                    OrderId = order.OrderId,
                    Status = "cancelled",
                    Note = "Đơn hàng đã được hủy.",
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Hủy đơn hàng thành công." });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
