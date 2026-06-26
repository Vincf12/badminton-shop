using Microsoft.EntityFrameworkCore;
using MyAPI.Infrastructure.Persistence;
using MyAPI.Domain.Entities;
using MyAPI.Application.DTOs;
using MyAPI.Services.Interfaces;

namespace MyAPI.Services
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly AppDbContext _context;

        public ProductVariantService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductVariantDto>> GetVariantsByProductAsync(int productId)
        {
            return await _context.ProductVariants
                .AsNoTracking()
                .Where(v => v.ProductId == productId)
                .Select(v => MapVariant(v))
                .ToListAsync();
        }

        public async Task<ServiceResult<ProductVariantDto>> GetVariantAsync(int id)
        {
            var variant = await _context.ProductVariants
                .AsNoTracking()
                .Where(v => v.VariantId == id)
                .Select(v => MapVariant(v))
                .FirstOrDefaultAsync();

            return variant == null
                ? ServiceResult<ProductVariantDto>.NotFound("Khong tim thay bien the san pham.")
                : ServiceResult<ProductVariantDto>.Ok(variant);
        }

        public async Task<ServiceResult<object>> CreateVariantAsync(int productId, ProductVariantUpsertDto dto)
        {
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == productId);
            if (!productExists)
            {
                return ServiceResult<object>.BadRequest("San pham khong ton tai.");
            }

            var sku = dto.Sku.Trim();
            var skuExists = await _context.ProductVariants.AnyAsync(v => v.Sku == sku);
            if (skuExists)
            {
                return ServiceResult<object>.BadRequest("SKU da ton tai.");
            }

            var variant = new ProductVariant
            {
                ProductId = productId,
                Sku = sku,
                Weight = NormalizeOptional(dto.Weight),
                GripSize = NormalizeOptional(dto.GripSize),
                Color = NormalizeOptional(dto.Color),
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                ImageUrl = NormalizeOptional(dto.ImageUrl)
            };

            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new
            {
                message = "Them bien the thanh cong.",
                variantId = variant.VariantId
            });
        }

        public async Task<ServiceResult<object>> UpdateVariantAsync(int id, ProductVariantUpsertDto dto)
        {
            var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.VariantId == id);
            if (variant == null)
            {
                return ServiceResult<object>.NotFound("Khong tim thay bien the san pham.");
            }

            var sku = dto.Sku.Trim();
            var skuExists = await _context.ProductVariants.AnyAsync(v => v.VariantId != id && v.Sku == sku);
            if (skuExists)
            {
                return ServiceResult<object>.BadRequest("SKU da ton tai.");
            }

            variant.Sku = sku;
            variant.Weight = NormalizeOptional(dto.Weight);
            variant.GripSize = NormalizeOptional(dto.GripSize);
            variant.Color = NormalizeOptional(dto.Color);
            variant.Price = dto.Price;
            variant.StockQuantity = dto.StockQuantity;
            variant.ImageUrl = NormalizeOptional(dto.ImageUrl);

            await _context.SaveChangesAsync();
            return ServiceResult<object>.OK("Cap nhat bien the thanh cong.");
        }

        public async Task<ServiceResult<object>> UpdateStockAsync(int id, UpdateStockDto dto)
        {
            var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.VariantId == id);
            if (variant == null)
            {
                return ServiceResult<object>.NotFound("Khong tim thay bien the san pham.");
            }

            variant.StockQuantity = dto.StockQuantity;
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Cap nhat ton kho thanh cong.");
        }

        public async Task<ServiceResult<object>> DeleteVariantAsync(int id)
        {
            var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.VariantId == id);
            if (variant == null)
            {
                return ServiceResult<object>.NotFound("Khong tim thay bien the san pham.");
            }

            _context.ProductVariants.Remove(variant);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Xoa bien the thanh cong.");
        }

        private static string? NormalizeOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static ProductVariantDto MapVariant(ProductVariant variant)
        {
            return new ProductVariantDto
            {
                VariantId = variant.VariantId,
                ProductId = variant.ProductId,
                Sku = variant.Sku,
                Weight = variant.Weight,
                GripSize = variant.GripSize,
                Color = variant.Color,
                Price = variant.Price,
                StockQuantity = variant.StockQuantity,
                ImageUrl = variant.ImageUrl
            };
        }
    }
}
