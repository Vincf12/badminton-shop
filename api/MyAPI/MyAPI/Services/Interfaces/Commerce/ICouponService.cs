using MyAPI.Application.DTOs;

namespace MyAPI.Services.Interfaces
{
    public interface ICouponService
    {
        Task<IEnumerable<CouponDto>> GetCouponsAsync();
        Task<ServiceResult<CouponDto>> GetCouponAsync(int id);
        Task<ServiceResult<CouponDto>> CreateCouponAsync(CouponUpsertDto dto);
        Task<ServiceResult<object>> UpdateCouponAsync(int id, CouponUpsertDto dto);
        Task<ServiceResult<object>> DeleteCouponAsync(int id);
        Task<ServiceResult<object>> ApplyCouponAsync(ApplyCouponDto dto);
    }
}
