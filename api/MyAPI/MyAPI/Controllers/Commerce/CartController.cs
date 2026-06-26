using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Application.DTOs;
using MyAPI.Services;
using MyAPI.Services.Interfaces;
using System.Security.Claims;

namespace MyAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out userId);
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

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var result = await _cartService.GetCartAsync(userId);
            return ToActionResult(result);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemDto dto)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var result = await _cartService.AddItemAsync(userId, dto);
            return ToActionResult(result);
        }

        [HttpPut("items/{cartItemId:int}")]
        public async Task<IActionResult> UpdateItemQuantity(int cartItemId, [FromBody] UpdateCartItemDto dto)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var result = await _cartService.UpdateItemQuantityAsync(userId, cartItemId, dto);
            return ToActionResult(result);
        }

        [HttpDelete("items/{cartItemId:int}")]
        public async Task<IActionResult> DeleteItem(int cartItemId)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var result = await _cartService.DeleteItemAsync(userId, cartItemId);
            return ToActionResult(result);
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var result = await _cartService.ClearCartAsync(userId);
            return ToActionResult(result);
        }
    }
}
