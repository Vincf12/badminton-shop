using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MyAPI.Controllers.Order
{
    [Route("api/payments")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
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

        private IActionResult ToActionResult<T>(ServiceResult<T> result)
        {
            if (result.Succeeded)
            {
                return Ok(result.Data);
            }

            if (result.StatusCode == StatusCodes.Status403Forbidden)
            {
                return Forbid();
            }

            return StatusCode(result.StatusCode, new { message = result.Message });
        }

        [HttpGet("order/{orderId:int}")]
        public async Task<IActionResult> GetPaymentByOrder(int orderId)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "KhÃ´ng thá»ƒ xÃ¡c Ä‘á»‹nh ngÆ°á»i dÃ¹ng hiá»‡n táº¡i." });
            }

            var result = await _paymentService.GetPaymentByOrderAsync(orderId, userId, IsAdminOrStaff());
            return ToActionResult(result);
        }

        [HttpPost("cod/create")]
        public async Task<IActionResult> CreateCodPayment([FromBody] CreateGatewayPaymentDto dto)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Khong the xac dinh nguoi dung hien tai." });
            }

            var result = await _paymentService.CreateCodPaymentAsync(dto, userId, IsAdminOrStaff());
            return ToActionResult(result);
        }

        [HttpPost("vnpay/create")]
        public async Task<IActionResult> CreateVnPayPayment([FromBody] CreateGatewayPaymentDto dto)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "KhÃ´ng thá»ƒ xÃ¡c Ä‘á»‹nh ngÆ°á»i dÃ¹ng hiá»‡n táº¡i." });
            }

            var callbackUrl = $"{Request.Scheme}://{Request.Host}/api/payments/vnpay/callback";
            var result = await _paymentService.CreateVnPayPaymentAsync(dto, userId, IsAdminOrStaff(), callbackUrl);
            return ToActionResult(result);
        }

        [AllowAnonymous]
        [HttpGet("vnpay/callback")]
        public async Task<IActionResult> VnPayCallback()
        {
            var result = await _paymentService.VnPayCallbackAsync(
                Request.Query["vnp_TxnRef"].ToString(),
                Request.Query["vnp_ResponseCode"].ToString(),
                Request.Query["vnp_TransactionNo"].ToString());

            return ToActionResult(result);
        }

        [HttpPost("momo/create")]
        public async Task<IActionResult> CreateMomoPayment([FromBody] CreateGatewayPaymentDto dto)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "KhÃ´ng thá»ƒ xÃ¡c Ä‘á»‹nh ngÆ°á»i dÃ¹ng hiá»‡n táº¡i." });
            }

            var fallbackPayUrl = $"{Request.Scheme}://{Request.Host}/api/payments/momo/callback";
            var result = await _paymentService.CreateMomoPaymentAsync(dto, userId, IsAdminOrStaff(), fallbackPayUrl);
            return ToActionResult(result);
        }

        [AllowAnonymous]
        [HttpPost("momo/callback")]
        public async Task<IActionResult> MomoCallback([FromBody] MomoCallbackDto dto)
        {
            var result = await _paymentService.MomoCallbackAsync(dto);
            return ToActionResult(result);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] UpdatePaymentStatusDto dto)
        {
            var result = await _paymentService.UpdatePaymentStatusAsync(id, dto);
            return ToActionResult(result);
        }
    }
}


