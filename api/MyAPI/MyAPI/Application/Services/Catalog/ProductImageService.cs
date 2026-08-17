using Microsoft.EntityFrameworkCore;

namespace MyAPI.Application.Services.Catalog
{
    public class ProductImageService : IProductImageService
    {
        private readonly AppDbContext _context;

        public ProductImageService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductImageDto>> GetImagesAsync(int productId)
        {
            return await _context.ProductImages
                .AsNoTracking()
                .Where(i => i.ProductId == productId)
                .OrderBy(i => i.SortOrder)
                .Select(i => MapImage(i))
                .ToListAsync();
        }

        public async Task<ServiceResult<object>> CreateImageAsync(int productId, ProductImageUpsertDto dto)
        {
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == productId);
            if (!productExists)
            {
                return ServiceResult<object>.BadRequest("Sản phẩm không tồn tại.");
            }

            if (dto.IsMain)
            {
                await ClearMainImagesAsync(productId);
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

            return ServiceResult<object>.Ok(new
            {
                message = "Thêm ảnh sản phẩm thành công.",
                imageId = productImage.ImageId
            });
        }

        public async Task<ServiceResult<object>> UpdateImageAsync(int id, ProductImageUpsertDto dto)
        {
            var image = await _context.ProductImages.FirstOrDefaultAsync(i => i.ImageId == id);
            if (image == null)
            {
                return ServiceResult<object>.NotFound("Không tìm thấy ảnh sản phẩm.");
            }

            if (dto.IsMain)
            {
                await ClearMainImagesAsync(image.ProductId, id);
            }

            image.ImageUrl = dto.ImageUrl.Trim();
            image.IsMain = dto.IsMain;
            image.SortOrder = dto.SortOrder;

            await _context.SaveChangesAsync();
            return ServiceResult<object>.OK("Cập nhật ảnh sản phẩm thành công.");
        }

        public async Task<ServiceResult<object>> SetMainImageAsync(int id)
        {
            var image = await _context.ProductImages.FirstOrDefaultAsync(i => i.ImageId == id);
            if (image == null)
            {
                return ServiceResult<object>.NotFound("Không tìm thấy ảnh sản phẩm.");
            }

            await ClearMainImagesAsync(image.ProductId, id);
            image.IsMain = true;
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Đã đặt làm ảnh chính.");
        }

        public async Task<ServiceResult<object>> DeleteImageAsync(int id)
        {
            var image = await _context.ProductImages.FirstOrDefaultAsync(i => i.ImageId == id);
            if (image == null)
            {
                return ServiceResult<object>.NotFound("Không tìm thấy ảnh sản phẩm.");
            }

            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Xóa ảnh sản phẩm thành công.");
        }

        private async Task ClearMainImagesAsync(int productId, int? excludedImageId = null)
        {
            var query = _context.ProductImages.Where(i => i.ProductId == productId && i.IsMain);
            if (excludedImageId.HasValue)
            {
                query = query.Where(i => i.ImageId != excludedImageId.Value);
            }

            var oldMainImages = await query.ToListAsync();
            foreach (var image in oldMainImages)
            {
                image.IsMain = false;
            }
        }

        private static ProductImageDto MapImage(ProductImage image)
        {
            return new ProductImageDto
            {
                ImageId = image.ImageId,
                ProductId = image.ProductId,
                ImageUrl = image.ImageUrl,
                IsMain = image.IsMain,
                SortOrder = image.SortOrder
            };
        }
    }
}


