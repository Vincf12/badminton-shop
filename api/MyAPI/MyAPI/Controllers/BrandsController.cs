using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MyAPI.Data;
using Microsoft.EntityFrameworkCore;
using MyAPI.Models; 
using MyAPI.Models.DTOs;

namespace MyAPI.Controllers
{
    [Route("api/brands")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BrandsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/brands
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetBrands()
        {
            var brands = await _context.Brands
                .AsNoTracking()
                .OrderBy(b => b.BrandName)
                .Select(b => new BrandDto
                {
                    BrandId = b.BrandId,
                    BrandName = b.BrandName
                })
                .ToListAsync();

            return Ok(brands);
        }

        // GET /api/brands/1
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BrandDto>> GetBrand(int id)
        {
            var brand = await _context.Brands
                .AsNoTracking()
                .Where(b => b.BrandId == id)
                .Select(b => new BrandDto
                {
                    BrandId = b.BrandId,
                    BrandName = b.BrandName
                })
                .FirstOrDefaultAsync();

            if (brand == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy thương hiệu."
                });
            }

            return Ok(brand);
        }
        // POST /api/brands
        [HttpPost]
        [Authorize(Roles = "admin,staff")]
        public async Task<ActionResult> CreateBrand([FromBody] BrandUpsertDto dto)
        {
            var brandName = dto.BrandName.Trim();
            bool exists = await _context.Brands.AnyAsync(b => b.BrandName == brandName);
            if (exists)
            {
                return BadRequest(new { message = "Thương hiệu đã tồn tại trên hệ thống" });
            }
            var brand = new Brand
            {
                BrandName = brandName
            };
           
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBrand), new { id = brand.BrandId }, new BrandDto 
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName 
            });
        }
        // PUT /api/brands/1
        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin,staff")]
        public async Task<ActionResult> UpdateBrand(int id, [FromBody] BrandUpsertDto dto)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.BrandId == id);
            if (brand == null)
            {
                return NotFound(new { message = "Thương hiệu không tồn tại trên hệ thống" });
            }
            var brandName = dto.BrandName.Trim();
            bool exists = await _context.Brands.AnyAsync(b => b.BrandName == brandName && b.BrandId != id);
            if (exists)
            {
                return BadRequest(new { message = "Thương hiệu đã tồn tại trên hệ thống" });
            }
            brand.BrandName = brandName;
            await _context.SaveChangesAsync();
         
            return Ok(new { message = "Cập nhật thương hiệu thành công" });
        }
        // DELETE /api/brands/1
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteBrand(int id)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.BrandId == id);
            if (brand == null)
            {
                return NotFound(new { message = "Thương hiệu không tồn tại trên hệ thống" });
            }
            bool hasProducts = await _context.Products.AnyAsync(p => p.BrandId == id);
            if (hasProducts)
            {
                return BadRequest(new { message = "Không thể xóa thương hiệu vì đang có sản phẩm liên quan" });
            }
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}