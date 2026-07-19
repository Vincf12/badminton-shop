using Microsoft.EntityFrameworkCore;

namespace MyAPI.Application.Services.Catalog
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;

        public ReviewService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<object>> GetProductReviewsAsync(int productId)
        {
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == productId);

            if (!productExists)
            {
                return ServiceResult<object>.NotFound("KhÃ´ng tÃ¬m tháº¥y sáº£n pháº©m.");
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

            return ServiceResult<object>.Ok(reviews);
        }

        public async Task<ServiceResult<object>> CreateReviewAsync(int productId, int userId, CreateReviewDto dto)
        {
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == productId);

            if (!productExists)
            {
                return ServiceResult<object>.NotFound("KhÃ´ng tÃ¬m tháº¥y sáº£n pháº©m.");
            }

            var exists = await _context.Reviews.AnyAsync(r => r.UserId == userId && r.ProductId == productId);

            if (exists)
            {
                return ServiceResult<object>.BadRequest("Báº¡n Ä‘Ã£ Ä‘Ã¡nh giÃ¡ sáº£n pháº©m nÃ y.");
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

            return ServiceResult<object>.Ok(new ReviewDto
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

        public async Task<ServiceResult<object>> UpdateReviewAsync(int id, int userId, UpdateReviewDto dto)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == id);

            if (review == null)
            {
                return ServiceResult<object>.NotFound("KhÃ´ng tÃ¬m tháº¥y Ä‘Ã¡nh giÃ¡.");
            }

            if (review.UserId != userId)
            {
                return ServiceResult<object>.Forbidden();
            }

            review.Rating = dto.Rating;
            review.Comment = string.IsNullOrWhiteSpace(dto.Comment) ? null : dto.Comment.Trim();

            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new { message = "Cáº­p nháº­t Ä‘Ã¡nh giÃ¡ thÃ nh cÃ´ng." });
        }

        public async Task<ServiceResult<object>> DeleteReviewAsync(int id, int userId, bool isAdmin)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == id);

            if (review == null)
            {
                return ServiceResult<object>.NotFound("KhÃ´ng tÃ¬m tháº¥y Ä‘Ã¡nh giÃ¡.");
            }

            if (!isAdmin && review.UserId != userId)
            {
                return ServiceResult<object>.Forbidden();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new { message = "XÃ³a Ä‘Ã¡nh giÃ¡ thÃ nh cÃ´ng." });
        }
    }
}


