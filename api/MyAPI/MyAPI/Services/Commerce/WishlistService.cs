using Microsoft.EntityFrameworkCore;
using MyAPI.Infrastructure.Persistence;
using MyAPI.Domain.Entities;
using MyAPI.Application.DTOs;
using MyAPI.Services.Interfaces;

namespace MyAPI.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly AppDbContext _context;

        public WishlistService(AppDbContext context)
        {
            _context = context;
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

        public async Task<ServiceResult<object>> GetWishlistAsync(int userId)
        {
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

            return ServiceResult<object>.Ok(new WishlistDto
            {
                WishlistId = wishlist.WishlistId,
                Items = items
            });
        }

        public async Task<ServiceResult<object>> AddWishlistItemAsync(int userId, AddWishlistItemDto dto)
        {
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == dto.ProductId);

            if (!productExists)
            {
                return ServiceResult<object>.NotFound("Không tìm thấy sản phẩm.");
            }

            var wishlist = await GetOrCreateWishlistAsync(userId);
            var exists = await _context.WishlistItems.AnyAsync(i => i.WishlistId == wishlist.WishlistId && i.ProductId == dto.ProductId);

            if (exists)
            {
                return ServiceResult<object>.BadRequest("Sản phẩm đã có trong danh sách yêu thích.");
            }

            _context.WishlistItems.Add(new WishlistItem
            {
                WishlistId = wishlist.WishlistId,
                ProductId = dto.ProductId
            });

            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new { message = "Đã thêm sản phẩm vào danh sách yêu thích." });
        }

        public async Task<ServiceResult<object>> DeleteWishlistItemAsync(int userId, int productId)
        {
            var wishlist = await GetOrCreateWishlistAsync(userId);
            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(i => i.WishlistId == wishlist.WishlistId && i.ProductId == productId);

            if (item == null)
            {
                return ServiceResult<object>.NotFound("Sản phẩm không có trong danh sách yêu thích.");
            }

            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new { message = "Đã xóa sản phẩm khỏi danh sách yêu thích." });
        }

        public async Task<ServiceResult<object>> CheckWishlistItemAsync(int userId, int productId)
        {
            var wishlist = await GetOrCreateWishlistAsync(userId);
            var exists = await _context.WishlistItems
                .AnyAsync(i => i.WishlistId == wishlist.WishlistId && i.ProductId == productId);

            return ServiceResult<object>.Ok(new
            {
                productId,
                isWishlisted = exists
            });
        }
    }
}
