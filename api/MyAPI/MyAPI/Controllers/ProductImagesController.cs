using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.DTOs;

namespace MyAPI.Controllers
{
    [ApiController]
    public class ProductImagesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductImagesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("api/products/{productId:int}/images")]
        public async Task<IActionResult> GetImages(int productId)
        {
            var images = await _context.ProductImages
                .AsNoTracking()
                .Where(i => i.ProductId == productId)
                .OrderBy(i => i.SortOrder)
                .Select(i => new ProductImageDto
                {
                    ImageId = i.ImageId,
                    ProductId = i.ProductId,
                    ImageUrl = i.ImageUrl,
                    IsMain = i.IsMain,
                    SortOrder = i.SortOrder
                })
                .ToListAsync();

            return Ok(images);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost("api/products/{productId:int}/images")]
        public async Task<IActionResult> CreateImage(int productId, [FromBody] ProductImageUpsertDto dto)
        {
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == productId);

            if (!productExists)
                return BadRequest(new { message = "Sản phẩm không tồn tại." });

            if (dto.IsMain)
            {
                var oldMainImages = await _context.ProductImages
                    .Where(i => i.ProductId == productId && i.IsMain)
                    .ToListAsync();

                foreach (var image in oldMainImages)
                    image.IsMain = false;
            }

            var productImage = new ProductImage
            {
                ProductId = productId,
                ImageUrl = dto.ImageUrl.Trim(),
                IsMain = dto.IsMain,
                SortOrder = dto.SortOrder
            };

            _context.ProductImages.Add(productImage);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Thêm ảnh sản phẩm thành công.",
                imageId = productImage.ImageId
            });
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("api/product-images/{id:int}")]
        public async Task<IActionResult> UpdateImage(int id, [FromBody] ProductImageUpsertDto dto)
        {
            var image = await _context.ProductImages.FirstOrDefaultAsync(i => i.ImageId == id);

            if (image == null)
                return NotFound(new { message = "Không tìm thấy ảnh sản phẩm." });

            if (dto.IsMain)
            {
                var oldMainImages = await _context.ProductImages
                    .Where(i => i.ProductId == image.ProductId && i.ImageId != id && i.IsMain)
                    .ToListAsync();

                foreach (var item in oldMainImages)
                    item.IsMain = false;
            }

            image.ImageUrl = dto.ImageUrl.Trim();
            image.IsMain = dto.IsMain;
            image.SortOrder = dto.SortOrder;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật ảnh sản phẩm thành công." });
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("api/product-images/{id:int}/main")]
        public async Task<IActionResult> SetMainImage(int id)
        {
            var image = await _context.ProductImages.FirstOrDefaultAsync(i => i.ImageId == id);

            if (image == null)
                return NotFound(new { message = "Không tìm thấy ảnh sản phẩm." });

            var oldMainImages = await _context.ProductImages
                .Where(i => i.ProductId == image.ProductId && i.ImageId != id && i.IsMain)
                .ToListAsync();

            foreach (var item in oldMainImages)
                item.IsMain = false;

            image.IsMain = true;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã đặt làm ảnh chính." });
        }

        [Authorize(Roles = "admin,staff")]
        [HttpDelete("api/product-images/{id:int}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.ProductImages.FirstOrDefaultAsync(i => i.ImageId == id);

            if (image == null)
                return NotFound(new { message = "Không tìm thấy ảnh sản phẩm." });

            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}