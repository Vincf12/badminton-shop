using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.DTOs;

namespace MyAPI.Controllers
{
    [ApiController]
    public class ProductSpecsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductSpecsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("api/products/{productId:int}/specs")]
        public async Task<IActionResult> GetSpecs(int productId)
        {
            var specs = await _context.ProductSpecs
                .AsNoTracking()
                .Where(s => s.ProductId == productId)
                .Select(s => new ProductSpecDto
                {
                    SpecId = s.SpecId,
                    ProductId = s.ProductId,
                    SpecName = s.SpecName,
                    SpecValue = s.SpecValue
                })
                .ToListAsync();

            return Ok(specs);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost("api/products/{productId:int}/specs")]
        public async Task<IActionResult> CreateSpec(int productId, [FromBody] ProductSpecUpsertDto dto)
        {
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == productId);

            if (!productExists)
                return BadRequest(new { message = "Sản phẩm không tồn tại." });

            var spec = new ProductSpec
            {
                ProductId = productId,
                SpecName = dto.SpecName.Trim(),
                SpecValue = dto.SpecValue.Trim()
            };

            _context.ProductSpecs.Add(spec);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Thêm thông số sản phẩm thành công.",
                specId = spec.SpecId
            });
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("api/product-specs/{id:int}")]
        public async Task<IActionResult> UpdateSpec(int id, [FromBody] ProductSpecUpsertDto dto)
        {
            var spec = await _context.ProductSpecs.FirstOrDefaultAsync(s => s.SpecId == id);

            if (spec == null)
                return NotFound(new { message = "Không tìm thấy thông số sản phẩm." });

            spec.SpecName = dto.SpecName.Trim();
            spec.SpecValue = dto.SpecValue.Trim();

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật thông số sản phẩm thành công." });
        }

        [Authorize(Roles = "admin,staff")]
        [HttpDelete("api/product-specs/{id:int}")]
        public async Task<IActionResult> DeleteSpec(int id)
        {
            var spec = await _context.ProductSpecs.FirstOrDefaultAsync(s => s.SpecId == id);

            if (spec == null)
                return NotFound(new { message = "Không tìm thấy thông số sản phẩm." });

            _context.ProductSpecs.Remove(spec);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}