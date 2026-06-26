using Microsoft.EntityFrameworkCore;
using MyAPI.Infrastructure.Persistence;
using MyAPI.Domain.Entities;
using MyAPI.Application.DTOs;
using MyAPI.Services.Interfaces;

namespace MyAPI.Services
{
    public class ProductSpecService : IProductSpecService
    {
        private readonly AppDbContext _context;

        public ProductSpecService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductSpecDto>> GetSpecsAsync(int productId)
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
                return ServiceResult<object>.BadRequest("San pham khong ton tai.");
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
                message = "Them thong so san pham thanh cong.",
                specId = spec.SpecId
            });
        }

        public async Task<ServiceResult<object>> UpdateSpecAsync(int id, ProductSpecUpsertDto dto)
        {
            var spec = await _context.ProductSpecs.FirstOrDefaultAsync(s => s.SpecId == id);
            if (spec == null)
            {
                return ServiceResult<object>.NotFound("Khong tim thay thong so san pham.");
            }

            spec.SpecName = dto.SpecName.Trim();
            spec.SpecValue = dto.SpecValue.Trim();

            await _context.SaveChangesAsync();
            return ServiceResult<object>.OK("Cap nhat thong so san pham thanh cong.");
        }

        public async Task<ServiceResult<object>> DeleteSpecAsync(int id)
        {
            var spec = await _context.ProductSpecs.FirstOrDefaultAsync(s => s.SpecId == id);
            if (spec == null)
            {
                return ServiceResult<object>.NotFound("Khong tim thay thong so san pham.");
            }

            _context.ProductSpecs.Remove(spec);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Xoa thong so san pham thanh cong.");
        }

        private static ProductSpecDto MapSpec(ProductSpec spec)
        {
            return new ProductSpecDto
            {
                SpecId = spec.SpecId,
                ProductId = spec.ProductId,
                SpecName = spec.SpecName,
                SpecValue = spec.SpecValue
            };
        }
    }
}
