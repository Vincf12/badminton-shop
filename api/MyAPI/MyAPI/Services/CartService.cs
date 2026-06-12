using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.DTOs;
using MyAPI.Services.Interfaces;

namespace MyAPI.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        private async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                return cart;
            }

            cart = new Cart
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return cart;
        }

        public async Task<ServiceResult<CartDto>> GetCartAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);

            var items = await (
                from item in _context.CartItems.AsNoTracking()
                join variant in _context.ProductVariants.AsNoTracking()
                    on item.VariantId equals variant.VariantId
                join product in _context.Products.AsNoTracking()
                    on variant.ProductId equals product.ProductId
                where item.CartId == cart.CartId
                select new CartItemDto
                {
                    CartItemId = item.CartItemId,
                    VariantId = variant.VariantId,
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ImageUrl = variant.ImageUrl,
                    Sku = variant.Sku,
                    Weight = variant.Weight,
                    GripSize = variant.GripSize,
                    Color = variant.Color,
                    Price = variant.Price,
                    Quantity = item.Quantity,
                    StockQuantity = variant.StockQuantity
                })
                .ToListAsync();

            return ServiceResult<CartDto>.Ok(new CartDto
            {
                CartId = cart.CartId,
                Items = items
            });
        }

        public async Task<ServiceResult<object>> AddItemAsync(int userId, AddCartItemDto dto)
        {
            if (dto.Quantity <= 0)
            {
                return ServiceResult<object>.BadRequest("Số lượng phải lớn hơn 0.");
            }

            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.VariantId == dto.VariantId);

            if (variant == null)
            {
                return ServiceResult<object>.NotFound("Biến thể sản phẩm không tồn tại.");
            }

            if (variant.StockQuantity <= 0)
            {
                return ServiceResult<object>.BadRequest("Sản phẩm đã hết hàng.");
            }

            var cart = await GetOrCreateCartAsync(userId);

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.VariantId == dto.VariantId);

            if (cartItem != null)
            {
                int newQuantity = cartItem.Quantity + dto.Quantity;

                if (newQuantity > variant.StockQuantity)
                {
                    return ServiceResult<object>.BadRequest($"Chỉ còn {variant.StockQuantity} sản phẩm trong kho.");
                }

                cartItem.Quantity = newQuantity;
            }
            else
            {
                if (dto.Quantity > variant.StockQuantity)
                {
                    return ServiceResult<object>.BadRequest($"Chỉ còn {variant.StockQuantity} sản phẩm trong kho.");
                }

                _context.CartItems.Add(new CartItem
                {
                    CartId = cart.CartId,
                    VariantId = dto.VariantId,
                    Quantity = dto.Quantity
                });
            }

            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new { message = "Đã thêm sản phẩm vào giỏ hàng." });
        }

        public async Task<ServiceResult<object>> UpdateItemQuantityAsync(int userId, int cartItemId, UpdateCartItemDto dto)
        {
            if (dto.Quantity <= 0)
            {
                return ServiceResult<object>.BadRequest("Số lượng phải lớn hơn 0.");
            }

            var cart = await GetOrCreateCartAsync(userId);

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.CartId == cart.CartId);

            if (cartItem == null)
            {
                return ServiceResult<object>.NotFound("Không tìm thấy sản phẩm trong giỏ hàng.");
            }

            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.VariantId == cartItem.VariantId);

            if (variant == null)
            {
                return ServiceResult<object>.NotFound("Biến thể sản phẩm không tồn tại.");
            }

            if (dto.Quantity > variant.StockQuantity)
            {
                return ServiceResult<object>.BadRequest("Số lượng vượt quá tồn kho.");
            }

            cartItem.Quantity = dto.Quantity;

            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new { message = "Cập nhật số lượng thành công." });
        }

        public async Task<ServiceResult<object>> DeleteItemAsync(int userId, int cartItemId)
        {
            var cart = await GetOrCreateCartAsync(userId);

            var item = await _context.CartItems
                .FirstOrDefaultAsync(i => i.CartItemId == cartItemId && i.CartId == cart.CartId);

            if (item == null)
            {
                return ServiceResult<object>.NotFound("Không tìm thấy sản phẩm trong giỏ hàng.");
            }

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new { message = "Đã xóa sản phẩm khỏi giỏ hàng." });
        }

        public async Task<ServiceResult<object>> ClearCartAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var items = await _context.CartItems.Where(i => i.CartId == cart.CartId).ToListAsync();

            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new { message = "Đã xóa tất cả sản phẩm khỏi giỏ hàng." });
        }
    }
}
