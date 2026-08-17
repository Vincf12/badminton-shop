using Microsoft.EntityFrameworkCore;

namespace MyAPI.Application.Services.Catalog
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly AppDbContext _context;

        public ProductVariantService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductVariantResponse>> GetVariantsByProductAsync(int productId)
        {
            return await _context.ProductVariants
                .AsNoTracking()
                .Where(v => v.ProductId == productId)
                .Select(v => MapVariant(v))
                .ToListAsync();
        }

        public async Task<ServiceResult<ProductVariantResponse>> GetVariantAsync(int id)
        {
            var variant = await _context.ProductVariants
                .AsNoTracking()
                .Where(v => v.VariantId == id)
                .Select(v => MapVariant(v))
                .FirstOrDefaultAsync();

            return variant == null
                ? ServiceResult<ProductVariantResponse>.NotFound("Không tìm thấy biến thể sản phẩm.")
                : ServiceResult<ProductVariantResponse>.Ok(variant);
        }

        public async Task<ServiceResult<object>> CreateVariantAsync(int productId, ProductVariantUpsertDto dto)
        {
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == productId);
            if (!productExists)
            {
                return ServiceResult<object>.BadRequest("Sản phẩm không tồn tại.");
            }

            var sku = dto.Sku.Trim();
            var skuExists = await _context.ProductVariants.AnyAsync(v => v.Sku == sku);
            if (skuExists)
            {
                return ServiceResult<object>.BadRequest("SKU đã tồn tại.");
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
                message = "Thêm biến thể thành công.",
                variantId = variant.VariantId
            });
        }

        public async Task<ServiceResult<object>> UpdateVariantAsync(int id, ProductVariantUpsertDto dto)
        {
            var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.VariantId == id);
            if (variant == null)
            {
                return ServiceResult<object>.NotFound("Không tìm thấy biến thể sản phẩm.");
            }

            var sku = dto.Sku.Trim();
            var skuExists = await _context.ProductVariants.AnyAsync(v => v.VariantId != id && v.Sku == sku);
            if (skuExists)
            {
                return ServiceResult<object>.BadRequest("SKU đã tồn tại.");
            }

            variant.Sku = sku;
            variant.Weight = NormalizeOptional(dto.Weight);
            variant.GripSize = NormalizeOptional(dto.GripSize);
            variant.Color = NormalizeOptional(dto.Color);
            variant.Price = dto.Price;
            variant.StockQuantity = dto.StockQuantity;
            variant.ImageUrl = NormalizeOptional(dto.ImageUrl);

            await _context.SaveChangesAsync();
            return ServiceResult<object>.OK("Cập nhật biến thể thành công.");
        }

        public async Task<ServiceResult<object>> UpdateStockAsync(int id, UpdateStockDto dto)
        {
            var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.VariantId == id);
            if (variant == null)
            {
                return ServiceResult<object>.NotFound("Không tìm thấy biến thể sản phẩm.");
            }

            variant.StockQuantity = dto.StockQuantity;
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Cập nhật tồn kho thành công.");
        }

        public async Task<ServiceResult<object>> DeleteVariantAsync(int id)
        {
            var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.VariantId == id);
            if (variant == null)
            {
                return ServiceResult<object>.NotFound("Không tìm thấy biến thể sản phẩm.");
            }

            _context.ProductVariants.Remove(variant);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Xóa biến thể thành công.");
        }

        private static string? NormalizeOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static ProductVariantResponse MapVariant(ProductVariant variant)
        {
            return new ProductVariantResponse
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


