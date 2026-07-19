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
                return ServiceResult<object>.BadRequest("San pham khong ton tai.");
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
                message = "Them anh san pham thanh cong.",
                imageId = productImage.ImageId
            });
        }

        public async Task<ServiceResult<object>> UpdateImageAsync(int id, ProductImageUpsertDto dto)
        {
            var image = await _context.ProductImages.FirstOrDefaultAsync(i => i.ImageId == id);
            if (image == null)
            {
                return ServiceResult<object>.NotFound("Khong tim thay anh san pham.");
            }

            if (dto.IsMain)
            {
                await ClearMainImagesAsync(image.ProductId, id);
            }

            image.ImageUrl = dto.ImageUrl.Trim();
            image.IsMain = dto.IsMain;
            image.SortOrder = dto.SortOrder;

            await _context.SaveChangesAsync();
            return ServiceResult<object>.OK("Cap nhat anh san pham thanh cong.");
        }

        public async Task<ServiceResult<object>> SetMainImageAsync(int id)
        {
            var image = await _context.ProductImages.FirstOrDefaultAsync(i => i.ImageId == id);
            if (image == null)
            {
                return ServiceResult<object>.NotFound("Khong tim thay anh san pham.");
            }

            await ClearMainImagesAsync(image.ProductId, id);
            image.IsMain = true;
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Da dat lam anh chinh.");
        }

        public async Task<ServiceResult<object>> DeleteImageAsync(int id)
        {
            var image = await _context.ProductImages.FirstOrDefaultAsync(i => i.ImageId == id);
            if (image == null)
            {
                return ServiceResult<object>.NotFound("Khong tim thay anh san pham.");
            }

            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Xoa anh san pham thanh cong.");
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


