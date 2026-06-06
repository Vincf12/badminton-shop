using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.DTOs;

namespace MyAPI.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.CategoryName)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET /api/categories/1
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .Where(c => c.CategoryId == id)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy danh mục."
                });
            }

            return Ok(category);
        }

        // POST /api/categories
        [Authorize(Roles = "admin,staff")]
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(
            [FromBody] CategoryUpsertDto dto)
        {
            bool exists = await _context.Categories
                .AnyAsync(c => c.CategoryName == dto.CategoryName);

            if (exists)
            {
                return BadRequest(new
                {
                    message = "Danh mục đã tồn tại."
                });
            }

            var category = new Category
            {
                CategoryName = dto.CategoryName.Trim()
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = category.CategoryId },
                new CategoryDto
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName
                });
        }

        // PUT /api/categories/1
        [Authorize(Roles = "admin,staff")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(
            int id,
            [FromBody] CategoryUpsertDto dto)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy danh mục."
                });
            }

            bool exists = await _context.Categories
                .AnyAsync(c =>
                    c.CategoryId != id &&
                    c.CategoryName == dto.CategoryName);

            if (exists)
            {
                return BadRequest(new
                {
                    message = "Tên danh mục đã tồn tại."
                });
            }

            category.CategoryName = dto.CategoryName.Trim();

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cập nhật danh mục thành công."
            });
        }

        // DELETE /api/categories/1
        [Authorize(Roles = "admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy danh mục."
                });
            }

            bool hasProducts = await _context.Products
                .AnyAsync(p => p.CategoryId == id);

            if (hasProducts)
            {
                return BadRequest(new
                {
                    message = "Không thể xóa danh mục vì đang có sản phẩm."
                });
            }

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}