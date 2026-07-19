using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MyAPI.Controllers.Customer
{
    [Route("api/wishlist")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
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
        public async Task<IActionResult> GetWishlist()
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "KhÃ´ng thá»ƒ xÃ¡c Ä‘á»‹nh ngÆ°á»i dÃ¹ng hiá»‡n táº¡i." });
            }

            var result = await _wishlistService.GetWishlistAsync(userId);
            return ToActionResult(result);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddWishlistItem([FromBody] AddWishlistItemDto dto)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "KhÃ´ng thá»ƒ xÃ¡c Ä‘á»‹nh ngÆ°á»i dÃ¹ng hiá»‡n táº¡i." });
            }

            var result = await _wishlistService.AddWishlistItemAsync(userId, dto);
            return ToActionResult(result);
        }

        [HttpDelete("items/{productId:int}")]
        public async Task<IActionResult> DeleteWishlistItem(int productId)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "KhÃ´ng thá»ƒ xÃ¡c Ä‘á»‹nh ngÆ°á»i dÃ¹ng hiá»‡n táº¡i." });
            }

            var result = await _wishlistService.DeleteWishlistItemAsync(userId, productId);
            return ToActionResult(result);
        }

        [HttpGet("check/{productId:int}")]
        public async Task<IActionResult> CheckWishlistItem(int productId)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "KhÃ´ng thá»ƒ xÃ¡c Ä‘á»‹nh ngÆ°á»i dÃ¹ng hiá»‡n táº¡i." });
            }

            var result = await _wishlistService.CheckWishlistItemAsync(userId, productId);
            return ToActionResult(result);
        }
    }
}


