using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.DTOs;

namespace MyAPI.Controllers
{
    [ApiController]
    public class ProductVariantsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductVariantsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("api/products/{productId:int}/variants")]
        public async Task<IActionResult> GetVariantsByProduct(int productId)
        {
            var variants = await _context.ProductVariants
                .AsNoTracking()
                .Where(v => v.ProductId == productId)
                .Select(v => new ProductVariantDto
                {
                    VariantId = v.VariantId,
                    ProductId = v.ProductId,
                    Sku = v.Sku,
                    Weight = v.Weight,
                    GripSize = v.GripSize,
                    Color = v.Color,
                    Price = v.Price,
                    StockQuantity = v.StockQuantity,
                    ImageUrl = v.ImageUrl
                })
                .ToListAsync();

            return Ok(variants);
        }

        [HttpGet("api/product-variants/{id:int}")]
        public async Task<IActionResult> GetVariant(int id)
        {
            var variant = await _context.ProductVariants
                .AsNoTracking()
                .Where(v => v.VariantId == id)
                .Select(v => new ProductVariantDto
                {
                    VariantId = v.VariantId,
                    ProductId = v.ProductId,
                    Sku = v.Sku,
                    Weight = v.Weight,
                    GripSize = v.GripSize,
                    Color = v.Color,
                    Price = v.Price,
                    StockQuantity = v.StockQuantity,
                    ImageUrl = v.ImageUrl
                })
                .FirstOrDefaultAsync();

            if (variant == null)
                return NotFound(new { message = "Không tìm thấy biến thể sản phẩm." });

            return Ok(variant);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost("api/products/{productId:int}/variants")]
        public async Task<IActionResult> CreateVariant(int productId, [FromBody] ProductVariantUpsertDto dto)
        {
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == productId);

            if (!productExists)
                return BadRequest(new { message = "Sản phẩm không tồn tại." });

            var sku = dto.Sku.Trim();

            var skuExists = await _context.ProductVariants.AnyAsync(v => v.Sku == sku);

            if (skuExists)
                return BadRequest(new { message = "SKU đã tồn tại." });

            var variant = new ProductVariant
            {
                ProductId = productId,
                Sku = sku,
                Weight = string.IsNullOrWhiteSpace(dto.Weight) ? null : dto.Weight.Trim(),
                GripSize = string.IsNullOrWhiteSpace(dto.GripSize) ? null : dto.GripSize.Trim(),
                Color = string.IsNullOrWhiteSpace(dto.Color) ? null : dto.Color.Trim(),
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                ImageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl) ? null : dto.ImageUrl.Trim()
            };

            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVariant), new { id = variant.VariantId }, new
            {
                message = "Thêm biến thể thành công.",
                variantId = variant.VariantId
            });
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("api/product-variants/{id:int}")]
        public async Task<IActionResult> UpdateVariant(int id, [FromBody] ProductVariantUpsertDto dto)
        {
            var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.VariantId == id);

            if (variant == null)
                return NotFound(new { message = "Không tìm thấy biến thể sản phẩm." });

            var sku = dto.Sku.Trim();

            var skuExists = await _context.ProductVariants
                .AnyAsync(v => v.VariantId != id && v.Sku == sku);

            if (skuExists)
                return BadRequest(new { message = "SKU đã tồn tại." });

            variant.Sku = sku;
            variant.Weight = string.IsNullOrWhiteSpace(dto.Weight) ? null : dto.Weight.Trim();
            variant.GripSize = string.IsNullOrWhiteSpace(dto.GripSize) ? null : dto.GripSize.Trim();
            variant.Color = string.IsNullOrWhiteSpace(dto.Color) ? null : dto.Color.Trim();
            variant.Price = dto.Price;
            variant.StockQuantity = dto.StockQuantity;
            variant.ImageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl) ? null : dto.ImageUrl.Trim();

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật biến thể thành công." });
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("api/product-variants/{id:int}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockDto dto)
        {
            var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.VariantId == id);

            if (variant == null)
                return NotFound(new { message = "Không tìm thấy biến thể sản phẩm." });

            variant.StockQuantity = dto.StockQuantity;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật tồn kho thành công." });
        }

        [Authorize(Roles = "admin,staff")]
        [HttpDelete("api/product-variants/{id:int}")]
        public async Task<IActionResult> DeleteVariant(int id)
        {
            var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.VariantId == id);

            if (variant == null)
                return NotFound(new { message = "Không tìm thấy biến thể sản phẩm." });

            _context.ProductVariants.Remove(variant);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}