using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.DTOs;
using MyAPI.Services.Interfaces;

namespace MyAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.CategoryName)
                .Select(c => MapCategory(c))
                .ToListAsync();
        }

        public async Task<ServiceResult<CategoryDto>> GetCategoryAsync(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .Where(c => c.CategoryId == id)
                .Select(c => MapCategory(c))
                .FirstOrDefaultAsync();

            return category == null
                ? ServiceResult<CategoryDto>.NotFound("Khong tim thay danh muc.")
                : ServiceResult<CategoryDto>.Ok(category);
        }

        public async Task<ServiceResult<CategoryDto>> CreateCategoryAsync(CategoryUpsertDto dto)
        {
            var categoryName = dto.CategoryName.Trim();
            var exists = await _context.Categories.AnyAsync(c => c.CategoryName == categoryName);
            if (exists)
            {
                return ServiceResult<CategoryDto>.BadRequest("Danh muc da ton tai.");
            }

            var category = new Category
            {
                CategoryName = categoryName
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return ServiceResult<CategoryDto>.Ok(MapCategory(category));
        }

        public async Task<ServiceResult<object>> UpdateCategoryAsync(int id, CategoryUpsertDto dto)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return ServiceResult<object>.NotFound("Khong tim thay danh muc.");
            }

            var categoryName = dto.CategoryName.Trim();
            var exists = await _context.Categories.AnyAsync(c =>
                c.CategoryId != id &&
                c.CategoryName == categoryName);

            if (exists)
            {
                return ServiceResult<object>.BadRequest("Ten danh muc da ton tai.");
            }

            category.CategoryName = categoryName;
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Cap nhat danh muc thanh cong.");
        }

        public async Task<ServiceResult<object>> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                return ServiceResult<object>.NotFound("Khong tim thay danh muc.");
            }

            var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
            if (hasProducts)
            {
                return ServiceResult<object>.BadRequest("Khong the xoa danh muc vi dang co san pham.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Xoa danh muc thanh cong.");
        }

        private static CategoryDto MapCategory(Category category)
        {
            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName
            };
        }
    }
}
