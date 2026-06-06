using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.DTOs;
using System.Security.Claims;

namespace MyAPI.Controllers
{
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewsController(AppDbContext context)
        {
            _context = context;
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out userId);
        }

        private bool IsAdmin()
        {
            return User.IsInRole("admin");
        }

        [AllowAnonymous]
        [HttpGet("api/products/{productId:int}/reviews")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == productId);

            if (!productExists)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm." });
            }

            var reviews = await (
                from review in _context.Reviews.AsNoTracking()
                join user in _context.Users.AsNoTracking()
                    on review.UserId equals user.UserId
                where review.ProductId == productId
                orderby review.CreatedAt descending
                select new ReviewDto
                {
                    ReviewId = review.ReviewId,
                    UserId = review.UserId,
                    FullName = user.FullName,
                    ProductId = review.ProductId,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt
                })
                .ToListAsync();

            return Ok(reviews);
        }

        [Authorize]
        [HttpPost("api/products/{productId:int}/reviews")]
        public async Task<IActionResult> CreateReview(int productId, [FromBody] CreateReviewDto dto)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var productExists = await _context.Products.AnyAsync(p => p.ProductId == productId);

            if (!productExists)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm." });
            }

            var exists = await _context.Reviews.AnyAsync(r => r.UserId == userId && r.ProductId == productId);

            if (exists)
            {
                return BadRequest(new { message = "Bạn đã đánh giá sản phẩm này." });
            }

            var review = new Review
            {
                UserId = userId,
                ProductId = productId,
                Rating = dto.Rating,
                Comment = string.IsNullOrWhiteSpace(dto.Comment) ? null : dto.Comment.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var user = await _context.Users.AsNoTracking().FirstAsync(u => u.UserId == userId);

            return Ok(new ReviewDto
            {
                ReviewId = review.ReviewId,
                UserId = review.UserId,
                FullName = user.FullName,
                ProductId = review.ProductId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            });
        }

        [Authorize]
        [HttpPut("api/reviews/{id:int}")]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] UpdateReviewDto dto)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == id);

            if (review == null)
            {
                return NotFound(new { message = "Không tìm thấy đánh giá." });
            }

            if (review.UserId != userId)
            {
                return Forbid();
            }

            review.Rating = dto.Rating;
            review.Comment = string.IsNullOrWhiteSpace(dto.Comment) ? null : dto.Comment.Trim();

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật đánh giá thành công." });
        }

        [Authorize]
        [HttpDelete("api/reviews/{id:int}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == id);

            if (review == null)
            {
                return NotFound(new { message = "Không tìm thấy đánh giá." });
            }

            if (!IsAdmin() && review.UserId != userId)
            {
                return Forbid();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
