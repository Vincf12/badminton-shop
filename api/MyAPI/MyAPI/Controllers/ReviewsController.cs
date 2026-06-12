using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Models.DTOs;
using MyAPI.Services;
using MyAPI.Services.Interfaces;
using System.Security.Claims;

namespace MyAPI.Controllers
{
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
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

        [AllowAnonymous]
        [HttpGet("api/products/{productId:int}/reviews")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var result = await _reviewService.GetProductReviewsAsync(productId);
            return ToActionResult(result);
        }

        [Authorize]
        [HttpPost("api/products/{productId:int}/reviews")]
        public async Task<IActionResult> CreateReview(int productId, [FromBody] CreateReviewDto dto)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var result = await _reviewService.CreateReviewAsync(productId, userId, dto);
            return ToActionResult(result);
        }

        [Authorize]
        [HttpPut("api/reviews/{id:int}")]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] UpdateReviewDto dto)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var result = await _reviewService.UpdateReviewAsync(id, userId, dto);
            return ToActionResult(result);
        }

        [Authorize]
        [HttpDelete("api/reviews/{id:int}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var result = await _reviewService.DeleteReviewAsync(id, userId, User.IsInRole("admin"));
            return ToActionResult(result);
        }
    }
}
