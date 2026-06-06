using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

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
                        join brand in _context.Brands.AsNoTracking()
                            on product.BrandId equals brand.BrandId
                        select new ProductListItemDto
                        {
                            ProductId = product.ProductId,
                            CategoryId = product.CategoryId,
                            CategoryName = category.CategoryName,
                            ProductName = product.ProductName,
                            BrandId = product.BrandId,
                            BrandName = brand.BrandName,

                            Price = _context.ProductVariants
                                .Where(v => v.ProductId == product.ProductId)
                                .Select(v => v.Price)
                                .FirstOrDefault(),

                            Stock = _context.ProductVariants
                                .Where(v => v.ProductId == product.ProductId)
                                .Sum(v => v.StockQuantity),

                            ImageUrl = _context.ProductImages
                                .Where(i => i.ProductId == product.ProductId && i.IsMain)
                                .OrderBy(i => i.SortOrder)
                                .Select(i => i.ImageUrl)
                                .FirstOrDefault(),

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
                    product.BrandName.Contains(search) ||
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
            var product = await MapProductAsync(id);

            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm." });
            }

            return Ok(product);
        }
        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<ProductDetailDto>> GetProductBySlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return BadRequest(new { message = "Slug không được để trống." });
            }
            slug = slug.Trim();

            var product = await (from p in _context.Products.AsNoTracking()
                                 join category in _context.Categories.AsNoTracking()
                                     on p.CategoryId equals category.CategoryId
                                 join brand in _context.Brands.AsNoTracking()
                                     on p.BrandId equals brand.BrandId
                                 where p.Slug == slug
                                 select new ProductDetailDto
                                 {
                                     ProductId = p.ProductId,
                                     CategoryId = p.CategoryId,
                                     CategoryName = category.CategoryName,
                                     ProductName = p.ProductName,
                                     BrandId = p.BrandId,
                                     BrandName = brand.BrandName,

                                     Price = _context.ProductVariants
                                         .Where(v => v.ProductId == p.ProductId)
                                         .Min(v => (decimal?)v.Price) ?? 0,

                                     Stock = _context.ProductVariants
                                         .Where(v => v.ProductId == p.ProductId)
                                         .Sum(v => (int?)v.StockQuantity) ?? 0,

                                     ImageUrl = _context.ProductImages
                                         .Where(i => i.ProductId == p.ProductId && i.IsMain)
                                         .OrderBy(i => i.SortOrder)
                                         .Select(i => i.ImageUrl)
                                         .FirstOrDefault(),

                                     Description = p.Description,
                                     CreatedAt = p.CreatedAt,
                                     UpdatedAt = p.UpdatedAt
                                 })
                                 .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm." });
            }

            return Ok(product);
        }
        [HttpPost]
        [Authorize(Roles = "admin,staff")]
        public async Task<ActionResult<ProductDetailDto>> CreateProduct([FromBody] ProductUpsertDto dto)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(category => category.CategoryId == dto.CategoryId);

            if (!categoryExists)
            {
                return BadRequest(new { message = "Danh mục không tồn tại." });
            }

            var brandExists = await _context.Brands
                .AnyAsync(brand => brand.BrandId == dto.BrandId);

            if (!brandExists)
            {
                return BadRequest(new { message = "Thương hiệu không tồn tại." });
            }

            var product = new Product
            {
                CategoryId = dto.CategoryId,
                BrandId = dto.BrandId,
                ProductName = dto.ProductName.Trim(),
                Slug = string.IsNullOrWhiteSpace(dto.Slug) ? null : dto.Slug.Trim(),
                ShortDescription = string.IsNullOrWhiteSpace(dto.ShortDescription) ? null : dto.ShortDescription.Trim(),
                Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "active" : dto.Status.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                _context.ProductImages.Add(new ProductImage
                {
                    ProductId = product.ProductId,
                    ImageUrl = dto.ImageUrl.Trim(),
                    IsMain = true,
                    SortOrder = 0
                });
            }

            _context.ProductVariants.Add(new ProductVariant
            {
                ProductId = product.ProductId,
                Price = dto.Price,
                StockQuantity = dto.Stock
            });

            await _context.SaveChangesAsync();

            var result = await MapProductAsync(product.ProductId);

            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin,staff")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpsertDto dto)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(item => item.ProductId == id);

            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm." });
            }

            var categoryExists = await _context.Categories
                .AnyAsync(category => category.CategoryId == dto.CategoryId);

            if (!categoryExists)
            {
                return BadRequest(new { message = "Danh mục không tồn tại." });
            }

            var brandExists = await _context.Brands
                .AnyAsync(brand => brand.BrandId == dto.BrandId);

            if (!brandExists)
            {
                return BadRequest(new { message = "Thương hiệu không tồn tại." });
            }

            product.CategoryId = dto.CategoryId;
            product.BrandId = dto.BrandId;
            product.ProductName = dto.ProductName.Trim();
            product.Slug = string.IsNullOrWhiteSpace(dto.Slug) ? null : dto.Slug.Trim();
            product.ShortDescription = string.IsNullOrWhiteSpace(dto.ShortDescription) ? null : dto.ShortDescription.Trim();
            product.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
            product.Status = string.IsNullOrWhiteSpace(dto.Status) ? "active" : dto.Status.Trim();
            product.UpdatedAt = DateTime.UtcNow;

            var variant = await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.ProductId == id);

            if (variant == null)
            {
                _context.ProductVariants.Add(new ProductVariant
                {
                    ProductId = id,
                    Price = dto.Price,
                    StockQuantity = dto.Stock
                });
            }
            else
            {
                variant.Price = dto.Price;
                variant.StockQuantity = dto.Stock;
            }

            var mainImage = await _context.ProductImages
                .FirstOrDefaultAsync(i => i.ProductId == id && i.IsMain);

            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                if (mainImage == null)
                {
                    _context.ProductImages.Add(new ProductImage
                    {
                        ProductId = id,
                        ImageUrl = dto.ImageUrl.Trim(),
                        IsMain = true,
                        SortOrder = 0
                    });
                }
                else
                {
                    mainImage.ImageUrl = dto.ImageUrl.Trim();
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(item => item.ProductId == id);

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

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductListItemDto>>> SearchProducts([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new { message = "Từ khóa tìm kiếm không được để trống." });
            }
            keyword = keyword.Trim();

            var products = await (from product in _context.Products.AsNoTracking()
                                  join category in _context.Categories.AsNoTracking()
                                      on product.CategoryId equals category.CategoryId
                                  join brand in _context.Brands.AsNoTracking()
                                      on product.BrandId equals brand.BrandId
                                  where product.ProductName.Contains(keyword) ||
                                        category.CategoryName.Contains(keyword) ||
                                        brand.BrandName.Contains(keyword)
                                  select new ProductListItemDto
                                  {
                                      ProductId = product.ProductId,
                                      CategoryId = product.CategoryId,
                                      CategoryName = category.CategoryName,
                                      ProductName = product.ProductName,
                                      BrandId = product.BrandId,
                                      BrandName = brand.BrandName,

                                      Price = _context.ProductVariants
                                          .Where(v => v.ProductId == product.ProductId)
                                          .OrderBy(v => v.VariantId)
                                          .Select(v => v.Price)
                                          .FirstOrDefault(),

                                      Stock = _context.ProductVariants
                                          .Where(v => v.ProductId == product.ProductId)
                                          .Sum(v => v.StockQuantity),

                                      ImageUrl = _context.ProductImages
                                          .Where(i => i.ProductId == product.ProductId && i.IsMain)
                                          .OrderBy(i => i.SortOrder)
                                          .Select(i => i.ImageUrl)
                                          .FirstOrDefault(),

                                      Description = product.Description,
                                      CreatedAt = product.CreatedAt
                                  })
                                  .OrderByDescending(product => product.CreatedAt)
                                  .ThenBy(product => product.ProductId)
                                  .ToListAsync();

            return Ok(products);
        }
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<ProductListItemDto>>> FilterProducts(
            [FromQuery] int? categoryId,
            [FromQuery] int? brandId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
            {
                return BadRequest(new { message = "Giá tối thiểu không được lớn hơn giá tối đa." });
            }
            var query = from product in _context.Products.AsNoTracking()
                        join category in _context.Categories.AsNoTracking()
                            on product.CategoryId equals category.CategoryId
                        join brand in _context.Brands.AsNoTracking()
                            on product.BrandId equals brand.BrandId
                        select new ProductListItemDto
                        {
                            ProductId = product.ProductId,
                            CategoryId = product.CategoryId,
                            CategoryName = category.CategoryName,
                            ProductName = product.ProductName,
                            BrandId = product.BrandId,
                            BrandName = brand.BrandName,

                            Price = _context.ProductVariants
                                .Where(v => v.ProductId == product.ProductId)
                                .OrderBy(v => v.VariantId)
                                .Select(v => v.Price)
                                .FirstOrDefault(),

                            Stock = _context.ProductVariants
                                .Where(v => v.ProductId == product.ProductId)
                                .Sum(v => v.StockQuantity),

                            ImageUrl = _context.ProductImages
                                .Where(i => i.ProductId == product.ProductId && i.IsMain)
                                .OrderBy(i => i.SortOrder)
                                .Select(i => i.ImageUrl)
                                .FirstOrDefault(),

                            Description = product.Description,
                            CreatedAt = product.CreatedAt
                        };

            if (categoryId.HasValue)
            {
                query = query.Where(product => product.CategoryId == categoryId.Value);
            }

            if (brandId.HasValue)
            {
                query = query.Where(product => product.BrandId == brandId.Value);
            }

            if (minPrice.HasValue)
            {
            query = query.Where(product => _context.ProductVariants.Any(v => v.ProductId == product.ProductId && v.Price >= minPrice.Value));
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(product => _context.ProductVariants.Any(v => v.ProductId == product.ProductId && v.Price <= maxPrice.Value));
            }

            var products = await query
                .OrderByDescending(product => product.CreatedAt)
                .ThenBy(product => product.ProductId)
                .ToListAsync();

            return Ok(products);
        }
        private async Task<ProductDetailDto?> MapProductAsync(int id)
        {
            return await (from product in _context.Products.AsNoTracking()
                          join category in _context.Categories.AsNoTracking()
                              on product.CategoryId equals category.CategoryId
                          join brand in _context.Brands.AsNoTracking()
                              on product.BrandId equals brand.BrandId
                          where product.ProductId == id
                          select new ProductDetailDto
                          {
                              ProductId = product.ProductId,
                              CategoryId = product.CategoryId,
                              CategoryName = category.CategoryName,
                              ProductName = product.ProductName,
                              BrandId = product.BrandId,
                              BrandName = brand.BrandName,

                              Price = _context.ProductVariants
                                  .Where(v => v.ProductId == product.ProductId)
                                  .OrderBy(v => v.VariantId)
                                  .Select(v => v.Price)
                                  .FirstOrDefault(),

                              Stock = _context.ProductVariants
                                  .Where(v => v.ProductId == product.ProductId)
                                  .Sum(v => v.StockQuantity),

                              ImageUrl = _context.ProductImages
                                  .Where(i => i.ProductId == product.ProductId && i.IsMain)
                                  .OrderBy(i => i.SortOrder)
                                  .Select(i => i.ImageUrl)
                                  .FirstOrDefault(),

                              Description = product.Description,
                              CreatedAt = product.CreatedAt,
                              UpdatedAt = product.UpdatedAt
                          })
                          .FirstOrDefaultAsync();
        }
    }
}