using Microsoft.EntityFrameworkCore;

namespace MyAPI.Application.Services.Catalog
{
    public class ProductSpecService : IProductSpecService
    {
        private readonly AppDbContext _context;

        public ProductSpecService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductSpecResponse>> GetSpecsAsync(int productId)
        {
            return await _context.ProductSpecs
                .AsNoTracking()
                .Where(s => s.ProductId == productId)
                .Select(s => MapSpec(s))
                .ToListAsync();
        }

        public async Task<ServiceResult<object>> CreateSpecAsync(int productId, ProductSpecUpsertDto dto)
        {
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == productId);
            if (!productExists)
            {
                return ServiceResult<object>.BadRequest("Sản phẩm không tồn tại.");
            }

            var spec = new ProductSpec
            {
                ProductId = productId,
                SpecName = dto.SpecName.Trim(),
                SpecValue = dto.SpecValue.Trim()
            };

            _context.ProductSpecs.Add(spec);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new
            {
                message = "Thêm thông số sản phẩm thành công.",
                specId = spec.SpecId
            });
        }

        public async Task<ServiceResult<object>> UpdateSpecAsync(int id, ProductSpecUpsertDto dto)
        {
            var spec = await _context.ProductSpecs.FirstOrDefaultAsync(s => s.SpecId == id);
            if (spec == null)
            {
                return ServiceResult<object>.NotFound("Không tìm thấy thông số sản phẩm.");
            }

            spec.SpecName = dto.SpecName.Trim();
            spec.SpecValue = dto.SpecValue.Trim();

            await _context.SaveChangesAsync();
            return ServiceResult<object>.OK("Cập nhật thông số sản phẩm thành công.");
        }

        public async Task<ServiceResult<object>> DeleteSpecAsync(int id)
        {
            var spec = await _context.ProductSpecs.FirstOrDefaultAsync(s => s.SpecId == id);
            if (spec == null)
            {
                return ServiceResult<object>.NotFound("Không tìm thấy thông số sản phẩm.");
            }

            _context.ProductSpecs.Remove(spec);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Xóa thông số sản phẩm thành công.");
        }

        private static ProductSpecResponse MapSpec(ProductSpec spec)
        {
            return new ProductSpecResponse
            {
                SpecId = spec.SpecId,
                ProductId = spec.ProductId,
                SpecName = spec.SpecName,
                SpecValue = spec.SpecValue
            };
        }
    }
}


