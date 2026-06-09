using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.DTOs;
using System.Security.Claims;
using MyAPI.Services.Interfaces;
using MyAPI.Services;

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
        private IActionResult HandleResult<T>(ServiceResult<T> result)
        {
            if (result.StatusCode == 204)
                return NoContent();

            if (!result.Succeeded)
            {
                return StatusCode(result.StatusCode, new
                {
                    message = result.Message
                });
            }

            if (result.Data == null)
            {
                return Ok(new
                {
                    message = result.Message
                });
            }

            return Ok(result.Data);
        }
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized();
            }

            var result = await _cartService.GetCartAsync(userId);
            return HandleResult(result);
        }
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemDto dto)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized();
            }

            var result = await _cartService.AddItemAsync(userId, dto);
            return HandleResult(result);
        }
        [HttpPut("items/{int:int}")]
        public async Task<IActionResult> UpdateItemQuantity(int cartItemId, [FromBody] UpdateCartItemDto dto)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Bạn cần đăng nhập để thực hiện hành động này." });
            }

            var result = await _cartService.UpdateItemQuantityAsync(userId, cartItemId, dto);
            return HandleResult(result);
        }
        [HttpDelete("items/{int:int}")]
        public async Task<IActionResult> DeleteItem(int cartItemId)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Bạn cần đăng nhập để thực hiện hành động này." });
            }

            var result = await _cartService.DeleteItemAsync(userId, cartItemId);
            return HandleResult(result);
        }
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Bạn cần đăng nhập để thực hiện hành động này." });
            }

            var result = await _cartService.ClearCartAsync(userId);
            return HandleResult(result);
        }
    }
}