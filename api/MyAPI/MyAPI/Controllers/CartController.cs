using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.DTOs;
using System.Security.Claims;

namespace MyAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out userId);
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

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

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

            return Ok(new CartDto
            {
                CartId = cart.CartId,
                Items = items
            });
        }
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemDto dto)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new
                {
                    message = "Không thể xác định người dùng hiện tại."
                });
            }

            // 1. Quantity > 0
            if (dto.Quantity <= 0)
            {
                return BadRequest(new
                {
                    message = "Số lượng phải lớn hơn 0."
                });
            }

            // 2. Variant tồn tại
            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.VariantId == dto.VariantId);

            if (variant == null)
            {
                return NotFound(new
                {
                    message = "Biến thể sản phẩm không tồn tại."
                });
            }

            // 3. Còn hàng không
            if (variant.StockQuantity <= 0)
            {
                return BadRequest(new
                {
                    message = "Sản phẩm đã hết hàng."
                });
            }

            var cart = await GetOrCreateCartAsync(userId);

            // 4. Đã có trong giỏ chưa
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(i =>
                    i.CartId == cart.CartId &&
                    i.VariantId == dto.VariantId);

            if (cartItem != null)
            {
                // 5. Tăng số lượng

                int newQuantity = cartItem.Quantity + dto.Quantity;

                // 6. Kiểm tra tồn kho
                if (newQuantity > variant.StockQuantity)
                {
                    return BadRequest(new
                    {
                        message = $"Chỉ còn {variant.StockQuantity} sản phẩm trong kho."
                    });
                }

                cartItem.Quantity = newQuantity;
            }
            else
            {
                // 7. Thêm mới

                if (dto.Quantity > variant.StockQuantity)
                {
                    return BadRequest(new
                    {
                        message = $"Chỉ còn {variant.StockQuantity} sản phẩm trong kho."
                    });
                }

                cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    VariantId = dto.VariantId,
                    Quantity = dto.Quantity
                };

                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Đã thêm sản phẩm vào giỏ hàng."
            });
        }

        [HttpPut("items/{id:int}")]
        public async Task<IActionResult> UpdateItemQuantity(int id, [FromBody] UpdateCartItemDto dto)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var cart = await GetOrCreateCartAsync(userId);

            var item = await _context.CartItems
                .FirstOrDefaultAsync(i => i.CartItemId == id && i.CartId == cart.CartId);

            if (item == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm trong giỏ hàng." });
            }

            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.VariantId == item.VariantId);

            if (variant == null)
            {
                return NotFound(new { message = "Biến thể sản phẩm không tồn tại." });
            }

            if (dto.Quantity > variant.StockQuantity)
            {
                return BadRequest(new { message = "Số lượng vượt quá tồn kho." });
            }

            item.Quantity = dto.Quantity;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật số lượng thành công." });
        }

        [HttpDelete("items/{id:int}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var cart = await GetOrCreateCartAsync(userId);

            var item = await _context.CartItems
                .FirstOrDefaultAsync(i => i.CartItemId == id && i.CartId == cart.CartId);

            if (item == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm trong giỏ hàng." });
            }

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var cart = await GetOrCreateCartAsync(userId);

            var items = await _context.CartItems
                .Where(i => i.CartId == cart.CartId)
                .ToListAsync();

            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}