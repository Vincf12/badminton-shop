using Microsoft.EntityFrameworkCore;

namespace MyAPI.Application.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        private static string GenerateOrderCode()
        {
            return $"ORD{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }

        public async Task<ServiceResult<object>> CreateOrderAsync(int userId, CreateOrderDto dto)
        {
            var paymentMethod = dto.PaymentMethod.Trim().ToLower();
            var validPaymentMethods = new[] { "cod", "vnpay", "momo" };

            if (!validPaymentMethods.Contains(paymentMethod))
            {
                return ServiceResult<object>.BadRequest("PhÆ°Æ¡ng thá»©c thanh toÃ¡n khÃ´ng há»£p lá»‡.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.AddressId == dto.AddressId && a.UserId == userId);

                if (address == null)
                {
                    return ServiceResult<object>.BadRequest("Äá»‹a chá»‰ giao hÃ ng khÃ´ng há»£p lá»‡.");
                }

                var cart = await _context.Carts
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    return ServiceResult<object>.BadRequest("Giá» hÃ ng trá»‘ng.");
                }

                var cartItems = await _context.CartItems
                    .Where(ci => ci.CartId == cart.CartId)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    return ServiceResult<object>.BadRequest("Giá» hÃ ng trá»‘ng.");
                }

                decimal totalAmount = 0;
                decimal shippingFee = 0;
                var orderDetails = new List<OrderDetail>();

                foreach (var item in cartItems)
                {
                    if (item.Quantity <= 0)
                    {
                        return ServiceResult<object>.BadRequest("Sá»‘ lÆ°á»£ng sáº£n pháº©m khÃ´ng há»£p lá»‡.");
                    }

                    var variant = await _context.ProductVariants
                        .FirstOrDefaultAsync(v => v.VariantId == item.VariantId);

                    if (variant == null)
                    {
                        return ServiceResult<object>.BadRequest("Biáº¿n thá»ƒ sáº£n pháº©m khÃ´ng tá»“n táº¡i.");
                    }

                    if (variant.StockQuantity < item.Quantity)
                    {
                        return ServiceResult<object>.BadRequest($"Sáº£n pháº©m {variant.Sku} khÃ´ng Ä‘á»§ tá»“n kho.");
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
                    var couponResult = await ApplyCouponAsync(dto.CouponCode, totalAmount);

                    if (!couponResult.Succeeded)
                    {
                        return ServiceResult<object>.BadRequest(couponResult.Message ?? "MÃ£ giáº£m giÃ¡ khÃ´ng há»£p lá»‡.");
                    }

                    discountAmount = couponResult.DiscountAmount;
                    couponId = couponResult.CouponId;
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

                _context.Payments.Add(new Payment
                {
                    OrderId = order.OrderId,
                    PaymentMethod = paymentMethod,
                    PaymentStatus = "pending",
                    Amount = finalAmount,
                    CreatedAt = DateTime.UtcNow
                });

                _context.OrderStatusHistories.Add(new OrderStatusHistory
                {
                    OrderId = order.OrderId,
                    Status = "pending",
                    Note = "ÄÆ¡n hÃ ng Ä‘Æ°á»£c táº¡o.",
                    CreatedAt = DateTime.UtcNow
                });

                _context.CartItems.RemoveRange(cartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ServiceResult<object>.Ok(new
                {
                    message = "Táº¡o Ä‘Æ¡n hÃ ng thÃ nh cÃ´ng.",
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

        public async Task<ServiceResult<object>> GetMyOrdersAsync(int userId)
        {
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

            return ServiceResult<object>.Ok(orders);
        }

        public async Task<ServiceResult<object>> GetAllOrdersAsync()
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

            return ServiceResult<object>.Ok(orders);
        }

        public async Task<ServiceResult<object>> GetOrderDetailAsync(int orderId, int currentUserId, bool isAdminOrStaff)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return ServiceResult<object>.NotFound("KhÃ´ng tÃ¬m tháº¥y Ä‘Æ¡n hÃ ng.");
            }

            if (!isAdminOrStaff && order.UserId != currentUserId)
            {
                return ServiceResult<object>.Forbidden();
            }

            var details = await (
                from d in _context.OrderDetails.AsNoTracking()
                join v in _context.ProductVariants.AsNoTracking()
                    on d.VariantId equals v.VariantId
                join p in _context.Products.AsNoTracking()
                    on v.ProductId equals p.ProductId
                where d.OrderId == orderId
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

            return ServiceResult<object>.Ok(new
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

        public async Task<ServiceResult<object>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto dto)
        {
            var validStatuses = new[] { "pending", "confirmed", "shipping", "completed", "cancelled" };

            if (!validStatuses.Contains(dto.Status))
            {
                return ServiceResult<object>.BadRequest("Tráº¡ng thÃ¡i Ä‘Æ¡n hÃ ng khÃ´ng há»£p lá»‡.");
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return ServiceResult<object>.NotFound("KhÃ´ng tÃ¬m tháº¥y Ä‘Æ¡n hÃ ng.");
            }

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

            return ServiceResult<object>.Ok(new { message = "Cáº­p nháº­t tráº¡ng thÃ¡i Ä‘Æ¡n hÃ ng thÃ nh cÃ´ng." });
        }

        public async Task<ServiceResult<object>> CancelOrderAsync(int orderId, int currentUserId, bool isAdminOrStaff)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    return ServiceResult<object>.NotFound("KhÃ´ng tÃ¬m tháº¥y Ä‘Æ¡n hÃ ng.");
                }

                if (!isAdminOrStaff && order.UserId != currentUserId)
                {
                    return ServiceResult<object>.Forbidden();
                }

                if (order.Status == "completed")
                {
                    return ServiceResult<object>.BadRequest("KhÃ´ng thá»ƒ há»§y Ä‘Æ¡n hÃ ng Ä‘Ã£ hoÃ n thÃ nh.");
                }

                if (order.Status == "cancelled")
                {
                    return ServiceResult<object>.BadRequest("ÄÆ¡n hÃ ng Ä‘Ã£ Ä‘Æ°á»£c há»§y trÆ°á»›c Ä‘Ã³.");
                }

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
                    Note = "ÄÆ¡n hÃ ng Ä‘Ã£ Ä‘Æ°á»£c há»§y.",
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ServiceResult<object>.Ok(new { message = "Há»§y Ä‘Æ¡n hÃ ng thÃ nh cÃ´ng." });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<CouponApplyResult> ApplyCouponAsync(string couponCode, decimal totalAmount)
        {
            var code = couponCode.Trim().ToUpper();
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code);

            if (coupon == null)
            {
                return CouponApplyResult.Fail("MÃ£ giáº£m giÃ¡ khÃ´ng tá»“n táº¡i.");
            }

            if (!coupon.IsActive)
            {
                return CouponApplyResult.Fail("MÃ£ giáº£m giÃ¡ Ä‘Ã£ bá»‹ khÃ³a.");
            }

            var now = DateTime.UtcNow;

            if (now < coupon.StartDate || now > coupon.EndDate)
            {
                return CouponApplyResult.Fail("MÃ£ giáº£m giÃ¡ khÃ´ng cÃ²n hiá»‡u lá»±c.");
            }

            if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value)
            {
                return CouponApplyResult.Fail("MÃ£ giáº£m giÃ¡ Ä‘Ã£ háº¿t lÆ°á»£t sá»­ dá»¥ng.");
            }

            if (totalAmount < coupon.MinimumOrderAmount)
            {
                return CouponApplyResult.Fail($"ÄÆ¡n hÃ ng pháº£i tá»« {coupon.MinimumOrderAmount:N0} VNÄ Ä‘á»ƒ dÃ¹ng mÃ£ nÃ y.");
            }

            decimal discountAmount;

            if (coupon.DiscountType == "percentage")
            {
                discountAmount = totalAmount * coupon.DiscountValue / 100;

                if (coupon.MaximumDiscountAmount.HasValue)
                {
                    discountAmount = Math.Min(discountAmount, coupon.MaximumDiscountAmount.Value);
                }
            }
            else if (coupon.DiscountType == "fixed")
            {
                discountAmount = coupon.DiscountValue;
            }
            else
            {
                return CouponApplyResult.Fail("Loáº¡i mÃ£ giáº£m giÃ¡ khÃ´ng há»£p lá»‡.");
            }

            if (discountAmount > totalAmount)
            {
                discountAmount = totalAmount;
            }

            coupon.UsedCount++;

            return CouponApplyResult.Ok(coupon.CouponId, discountAmount);
        }

        private class CouponApplyResult
        {
            public bool Succeeded { get; init; }
            public string? Message { get; init; }
            public int? CouponId { get; init; }
            public decimal DiscountAmount { get; init; }

            public static CouponApplyResult Ok(int couponId, decimal discountAmount)
            {
                return new CouponApplyResult
                {
                    Succeeded = true,
                    CouponId = couponId,
                    DiscountAmount = discountAmount
                };
            }

            public static CouponApplyResult Fail(string message)
            {
                return new CouponApplyResult
                {
                    Succeeded = false,
                    Message = message
                };
            }
        }
    }
}


