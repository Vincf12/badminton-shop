using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.DTOs;
using System.Security.Claims;

namespace MyAPI.Controllers
{
    [Route("api/wishlist")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WishlistController(AppDbContext context)
        {
            _context = context;
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out userId);
        }

        private async Task<Wishlist> GetOrCreateWishlistAsync(int userId)
        {
            var wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist != null)
            {
                return wishlist;
            }

            wishlist = new Wishlist
            {
                UserId = userId
            };

            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();

            return wishlist;
        }

        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var wishlist = await GetOrCreateWishlistAsync(userId);

            var items = await (
                from item in _context.WishlistItems.AsNoTracking()
                join product in _context.Products.AsNoTracking()
                    on item.ProductId equals product.ProductId
                join category in _context.Categories.AsNoTracking()
                    on product.CategoryId equals category.CategoryId
                join brand in _context.Brands.AsNoTracking()
                    on product.BrandId equals brand.BrandId
                where item.WishlistId == wishlist.WishlistId
                select new WishlistItemDto
                {
                    WishlistItemId = item.WishlistItemId,
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    CategoryName = category.CategoryName,
                    BrandName = brand.BrandName,
                    Price = _context.ProductVariants
                        .Where(v => v.ProductId == product.ProductId)
                        .OrderBy(v => v.VariantId)
                        .Select(v => v.Price)
                        .FirstOrDefault(),
                    Stock = _context.ProductVariants
                        .Where(v => v.ProductId == product.ProductId)
                        .Sum(v => v.StockQuantity),
                    ImageUrl = _context.ProductImages
                        .Where(i => i.ProductId == product.ProductId && i.IsMain)
                        .OrderBy(i => i.SortOrder)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(new WishlistDto
            {
                WishlistId = wishlist.WishlistId,
                Items = items
            });
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddWishlistItem([FromBody] AddWishlistItemDto dto)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var productExists = await _context.Products.AnyAsync(p => p.ProductId == dto.ProductId);

            if (!productExists)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm." });
            }

            var wishlist = await GetOrCreateWishlistAsync(userId);
            var exists = await _context.WishlistItems.AnyAsync(i => i.WishlistId == wishlist.WishlistId && i.ProductId == dto.ProductId);

            if (exists)
            {
                return BadRequest(new { message = "Sản phẩm đã có trong danh sách yêu thích." });
            }

            _context.WishlistItems.Add(new WishlistItem
            {
                WishlistId = wishlist.WishlistId,
                ProductId = dto.ProductId
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã thêm sản phẩm vào danh sách yêu thích." });
        }

        [HttpDelete("items/{productId:int}")]
        public async Task<IActionResult> DeleteWishlistItem(int productId)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var wishlist = await GetOrCreateWishlistAsync(userId);
            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(i => i.WishlistId == wishlist.WishlistId && i.ProductId == productId);

            if (item == null)
            {
                return NotFound(new { message = "Sản phẩm không có trong danh sách yêu thích." });
            }

            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("check/{productId:int}")]
        public async Task<IActionResult> CheckWishlistItem(int productId)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var wishlist = await GetOrCreateWishlistAsync(userId);
            var exists = await _context.WishlistItems
                .AnyAsync(i => i.WishlistId == wishlist.WishlistId && i.ProductId == productId);

            return Ok(new
            {
                productId,
                isWishlisted = exists
            });
        }
    }
}
