using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Models.DTOs;
using MyAPI.Services;
using MyAPI.Services.Interfaces;
using System.Security.Claims;

namespace MyAPI.Controllers
{
    [Route("api/orders")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
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

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var result = await _orderService.CreateOrderAsync(userId, dto);
            return ToActionResult(result);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyOrders()
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var result = await _orderService.GetMyOrdersAsync(userId);
            return ToActionResult(result);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var result = await _orderService.GetAllOrdersAsync();
            return ToActionResult(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrderDetail(int id)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var result = await _orderService.GetOrderDetailAsync(id, userId, IsAdminOrStaff());
            return ToActionResult(result);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, dto);
            return ToActionResult(result);
        }

        [HttpPut("{id:int}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var result = await _orderService.CancelOrderAsync(id, userId, IsAdminOrStaff());
            return ToActionResult(result);
        }
    }
}
