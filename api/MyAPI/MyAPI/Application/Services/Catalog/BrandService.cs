using Microsoft.EntityFrameworkCore;

namespace MyAPI.Application.Services.Catalog
{
    public class BrandService : IBrandService
    {
        private readonly AppDbContext _context;

        public BrandService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BrandDto>> GetBrandsAsync()
        {
            return await _context.Brands
                .AsNoTracking()
                .OrderBy(b => b.BrandName)
                .Select(b => MapBrand(b))
                .ToListAsync();
        }

        public async Task<ServiceResult<BrandDto>> GetBrandAsync(int id)
        {
            var brand = await _context.Brands
                .AsNoTracking()
                .Where(b => b.BrandId == id)
                .Select(b => MapBrand(b))
                .FirstOrDefaultAsync();

            return brand == null
                ? ServiceResult<BrandDto>.NotFound("Thương hiệu không tồn tại trên hệ thống.")
                : ServiceResult<BrandDto>.Ok(brand);
        }

        public async Task<ServiceResult<BrandDto>> CreateBrandAsync(BrandUpsertDto dto)
        {
            var brandName = dto.BrandName.Trim();
            var exists = await _context.Brands.AnyAsync(b => b.BrandName == brandName);
            if (exists)
            {
                return ServiceResult<BrandDto>.BadRequest("Thương hiệu đã tồn tại trên hệ thống.");
            }

            var brand = new Brand
            {
                BrandName = brandName
            };

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return ServiceResult<BrandDto>.Ok(MapBrand(brand));
        }

        public async Task<ServiceResult<object>> UpdateBrandAsync(int id, BrandUpsertDto dto)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.BrandId == id);
            if (brand == null)
            {
                return ServiceResult<object>.NotFound("Thương hiệu không tồn tại trên hệ thống.");
            }

            var brandName = dto.BrandName.Trim();
            var exists = await _context.Brands.AnyAsync(b => b.BrandName == brandName && b.BrandId != id);
            if (exists)
            {
                return ServiceResult<object>.BadRequest("Thương hiệu đã tồn tại trên hệ thống.");
            }

            brand.BrandName = brandName;
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("ập nhật thương hiệu thành công.");
        }

        public async Task<ServiceResult<object>> DeleteBrandAsync(int id)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.BrandId == id);
            if (brand == null)
            {
                return ServiceResult<object>.NotFound("Thương hiệu không tồn tại trên hệ thống.");
            }

            var hasProducts = await _context.Products.AnyAsync(p => p.BrandId == id);
            if (hasProducts)
            {
                return ServiceResult<object>.BadRequest("Không thể xóa thương hiệu vì đang có sản phẩm liên quan.");
            }

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Xóa thương hiệu thành công.");
        }

        private static BrandDto MapBrand(Brand brand)
        {
            return new BrandDto
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName
            };
        }
    }
}


