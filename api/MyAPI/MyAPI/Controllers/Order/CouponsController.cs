using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyAPI.Controllers.Order
{
    [Route("api/coupons")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetCoupons()
        {
            var coupons = await _couponService.GetCouponsAsync();
            return Ok(coupons);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCoupon(int id)
        {
            var result = await _couponService.GetCouponAsync(id);
            return ToActionResult(result);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromBody] CouponUpsertDto dto)
        {
            var result = await _couponService.CreateCouponAsync(dto);
            if (!result.Succeeded)
            {
                return ToActionResult(result);
            }

            return CreatedAtAction(nameof(GetCoupon), new { id = result.Data!.CouponId }, result.Data);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCoupon(int id, [FromBody] CouponUpsertDto dto)
        {
            var result = await _couponService.UpdateCouponAsync(id, dto);
            return ToActionResult(result);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var result = await _couponService.DeleteCouponAsync(id);
            if (!result.Succeeded)
            {
                return ToActionResult(result);
            }

            return NoContent();
        }

        [Authorize]
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponDto dto)
        {
            var result = await _couponService.ApplyCouponAsync(dto);
            return ToActionResult(result);
        }

        private IActionResult ToActionResult<T>(ServiceResult<T> result)
        {
            if (result.Succeeded)
            {
                return result.Data != null ? Ok(result.Data) : Ok(new { message = result.Message });
            }

            return StatusCode(result.StatusCode, new { message = result.Message });
        }
    }
}


