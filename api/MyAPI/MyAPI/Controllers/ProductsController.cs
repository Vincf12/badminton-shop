using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.DTOs;

namespace MyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductListItemDto>>> GetProducts(
            [FromQuery] string? search,
            [FromQuery] int? categoryId)
        {
            var query = from product in _context.Products.AsNoTracking()
                        join category in _context.Categories.AsNoTracking()
                            on product.CategoryId equals category.CategoryId
                        select new ProductListItemDto
                        {
                            ProductId = product.ProductId,
                            CategoryId = product.CategoryId,
                            CategoryName = category.CategoryName,
                            ProductName = product.ProductName,
                            Brand = product.Brand,
                            Price = product.Price,
                            Stock = product.Stock,
                            ImageUrl = product.ImageUrl,
                            Description = product.Description,
                            CreatedAt = product.CreatedAt
                        };

            if (categoryId.HasValue)
            {
                query = query.Where(product => product.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(product =>
                    product.ProductName.Contains(search) ||
                    (product.Brand != null && product.Brand.Contains(search)) ||
                    product.CategoryName.Contains(search));
            }

            var products = await query
                .OrderByDescending(product => product.CreatedAt)
                .ThenBy(product => product.ProductId)
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDetailDto>> GetProduct(int id)
        {
            var product = await (from item in _context.Products.AsNoTracking()
                                 join category in _context.Categories.AsNoTracking()
                                     on item.CategoryId equals category.CategoryId
                                 where item.ProductId == id
                                 select new ProductDetailDto
                                 {
                                     ProductId = item.ProductId,
                                     CategoryId = item.CategoryId,
                                     CategoryName = category.CategoryName,
                                     ProductName = item.ProductName,
                                     Brand = item.Brand,
                                     Price = item.Price,
                                     Stock = item.Stock,
                                     ImageUrl = item.ImageUrl,
                                     Description = item.Description,
                                     CreatedAt = item.CreatedAt,
                                     UpdatedAt = item.UpdatedAt
                                 })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm." });
            }

            return Ok(product);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .OrderBy(category => category.CategoryName)
                .Select(category => new CategoryDto
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName
                })
                .ToListAsync();

            return Ok(categories);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDetailDto>> CreateProduct([FromBody] ProductUpsertDto dto)
        {
            var categoryExists = await _context.Categories.AnyAsync(category => category.CategoryId == dto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest(new { message = "Danh mục không tồn tại." });
            }

            var product = new Product
            {
                CategoryId = dto.CategoryId,
                ProductName = dto.ProductName.Trim(),
                Brand = string.IsNullOrWhiteSpace(dto.Brand) ? null : dto.Brand.Trim(),
                Price = dto.Price,
                Stock = dto.Stock,
                ImageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl) ? null : dto.ImageUrl.Trim(),
                Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, await MapProductAsync(product.ProductId));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpsertDto dto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(item => item.ProductId == id);
            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm." });
            }

            var categoryExists = await _context.Categories.AnyAsync(category => category.CategoryId == dto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest(new { message = "Danh mục không tồn tại." });
            }

            product.CategoryId = dto.CategoryId;
            product.ProductName = dto.ProductName.Trim();
            product.Brand = string.IsNullOrWhiteSpace(dto.Brand) ? null : dto.Brand.Trim();
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.ImageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl) ? null : dto.ImageUrl.Trim();
            product.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(item => item.ProductId == id);
            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm." });
            }

            _context.Products.Remove(product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = "Không thể xóa sản phẩm vì đang được tham chiếu bởi dữ liệu khác." });
            }

            return NoContent();
        }

        private async Task<ProductDetailDto?> MapProductAsync(int id)
        {
            return await (from item in _context.Products.AsNoTracking()
                          join category in _context.Categories.AsNoTracking()
                              on item.CategoryId equals category.CategoryId
                          where item.ProductId == id
                          select new ProductDetailDto
                          {
                              ProductId = item.ProductId,
                              CategoryId = item.CategoryId,
                              CategoryName = category.CategoryName,
                              ProductName = item.ProductName,
                              Brand = item.Brand,
                              Price = item.Price,
                              Stock = item.Stock,
                              ImageUrl = item.ImageUrl,
                              Description = item.Description,
                              CreatedAt = item.CreatedAt,
                              UpdatedAt = item.UpdatedAt
                          }).FirstOrDefaultAsync();
        }
    }
}