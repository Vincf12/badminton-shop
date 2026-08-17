using Microsoft.EntityFrameworkCore;

namespace MyAPI.Application.Services.Order
{
    public class CouponService : ICouponService
    {
        private readonly AppDbContext _context;

        public CouponService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CouponDto>> GetCouponsAsync()
        {
            return await _context.Coupons
                .AsNoTracking()
                .OrderByDescending(c => c.CouponId)
                .Select(c => MapCoupon(c))
                .ToListAsync();
        }

        public async Task<ServiceResult<CouponDto>> GetCouponAsync(int id)
        {
            var coupon = await _context.Coupons
                .AsNoTracking()
                .Where(c => c.CouponId == id)
                .Select(c => MapCoupon(c))
                .FirstOrDefaultAsync();

            return coupon == null
                ? ServiceResult<CouponDto>.NotFound("Không tìm thấy coupon.")
                : ServiceResult<CouponDto>.Ok(coupon);
        }

        public async Task<ServiceResult<CouponDto>> CreateCouponAsync(CouponUpsertDto dto)
        {
            var code = dto.Code.Trim().ToUpperInvariant();
            var exists = await _context.Coupons.AnyAsync(c => c.Code == code);
            if (exists)
            {
                return ServiceResult<CouponDto>.BadRequest("Mã coupon đã tồn tại.");
            }

            var coupon = new Coupon();
            ApplyDto(coupon, dto, code);

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();

            return ServiceResult<CouponDto>.Ok(MapCoupon(coupon));
        }

        public async Task<ServiceResult<object>> UpdateCouponAsync(int id, CouponUpsertDto dto)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.CouponId == id);
            if (coupon == null)
            {
                return ServiceResult<object>.NotFound("Không tìm thấy coupon.");
            }

            var code = dto.Code.Trim().ToUpperInvariant();
            var exists = await _context.Coupons.AnyAsync(c => c.CouponId != id && c.Code == code);
            if (exists)
            {
                return ServiceResult<object>.BadRequest("Mã coupon đã tồn tại.");
            }

            ApplyDto(coupon, dto, code);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Cập nhật coupon thành công.");
        }

        public async Task<ServiceResult<object>> DeleteCouponAsync(int id)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.CouponId == id);
            if (coupon == null)
            {
                return ServiceResult<object>.NotFound("Không tìm thấy coupon.");
            }

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Xóa coupon thành công.");
        }

        public async Task<ServiceResult<object>> ApplyCouponAsync(ApplyCouponDto dto)
        {
            var code = dto.Code.Trim().ToUpperInvariant();
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code);

            if (coupon == null)
            {
                return ServiceResult<object>.BadRequest("Mã giảm giá không tồn tại.");
            }

            if (!coupon.IsActive)
            {
                return ServiceResult<object>.BadRequest("Mã giảm giá đã bị khóa.");
            }

            var now = DateTime.UtcNow;
            if (now < coupon.StartDate || now > coupon.EndDate)
            {
                return ServiceResult<object>.BadRequest("Mã giảm giá đã hết hạn hoặc chưa có hiệu lực.");
            }

            if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value)
            {
                return ServiceResult<object>.BadRequest("Mã giảm giá đã hết lượt sử dụng.");
            }

            if (dto.OrderAmount < coupon.MinimumOrderAmount)
            {
                return ServiceResult<object>.BadRequest($"Đơn hàng phải từ {coupon.MinimumOrderAmount:N0} VND.");
            }

            var discountAmount = coupon.DiscountType == "percentage"
                ? dto.OrderAmount * coupon.DiscountValue / 100
                : coupon.DiscountValue;

            if (coupon.DiscountType == "percentage" && coupon.MaximumDiscountAmount.HasValue)
            {
                discountAmount = Math.Min(discountAmount, coupon.MaximumDiscountAmount.Value);
            }

            return ServiceResult<object>.Ok(new
            {
                couponId = coupon.CouponId,
                code = coupon.Code,
                discountAmount,
                finalAmount = dto.OrderAmount - discountAmount
            });
        }

        private static void ApplyDto(Coupon coupon, CouponUpsertDto dto, string code)
        {
            coupon.Code = code;
            coupon.CouponName = dto.CouponName.Trim();
            coupon.DiscountType = dto.DiscountType;
            coupon.DiscountValue = dto.DiscountValue;
            coupon.MinimumOrderAmount = dto.MinimumOrderAmount;
            coupon.MaximumDiscountAmount = dto.MaximumDiscountAmount;
            coupon.UsageLimit = dto.UsageLimit;
            coupon.StartDate = dto.StartDate;
            coupon.EndDate = dto.EndDate;
            coupon.IsActive = dto.IsActive;
        }

        private static CouponDto MapCoupon(Coupon coupon)
        {
            return new CouponDto
            {
                CouponId = coupon.CouponId,
                Code = coupon.Code,
                CouponName = coupon.CouponName,
                DiscountType = coupon.DiscountType,
                DiscountValue = coupon.DiscountValue,
                MinimumOrderAmount = coupon.MinimumOrderAmount,
                MaximumDiscountAmount = coupon.MaximumDiscountAmount,
                UsageLimit = coupon.UsageLimit,
                UsedCount = coupon.UsedCount,
                StartDate = coupon.StartDate,
                EndDate = coupon.EndDate,
                IsActive = coupon.IsActive
            };
        }
    }
}


