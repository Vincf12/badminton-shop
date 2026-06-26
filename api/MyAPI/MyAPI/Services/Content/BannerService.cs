using Microsoft.EntityFrameworkCore;
using MyAPI.Infrastructure.Persistence;
using MyAPI.Domain.Entities;
using MyAPI.Application.DTOs;
using MyAPI.Services.Interfaces;

namespace MyAPI.Services
{
    public class BannerService : IBannerService
    {
        private readonly AppDbContext _context;

        public BannerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BannerDto>> GetBannersAsync()
        {
            return await _context.Banners
                .AsNoTracking()
                .OrderBy(b => b.Position)
                .ThenBy(b => b.DisplayOrder)
                .ThenByDescending(b => b.CreatedAt)
                .Select(b => MapBanner(b))
                .ToListAsync();
        }

        public async Task<ServiceResult<BannerDto>> GetBannerAsync(long id)
        {
            var banner = await _context.Banners
                .AsNoTracking()
                .Where(b => b.BannerId == id)
                .Select(b => MapBanner(b))
                .FirstOrDefaultAsync();

            return banner == null
                ? ServiceResult<BannerDto>.NotFound("Khong tim thay banner.")
                : ServiceResult<BannerDto>.Ok(banner);
        }

        public async Task<ServiceResult<BannerDto>> CreateBannerAsync(BannerUpsertDto dto)
        {
            var validationMessage = ValidateBanner(dto);
            if (validationMessage != null)
            {
                return ServiceResult<BannerDto>.BadRequest(validationMessage);
            }

            var banner = new Banner();
            ApplyUpsert(banner, dto);
            banner.CreatedAt = DateTime.UtcNow;
            banner.Version = 1;

            _context.Banners.Add(banner);
            await _context.SaveChangesAsync();

            return ServiceResult<BannerDto>.Ok(MapBanner(banner));
        }

        public async Task<ServiceResult<object>> UpdateBannerAsync(long id, BannerUpsertDto dto)
        {
            var validationMessage = ValidateBanner(dto);
            if (validationMessage != null)
            {
                return ServiceResult<object>.BadRequest(validationMessage);
            }

            var banner = await _context.Banners.FirstOrDefaultAsync(b => b.BannerId == id);
            if (banner == null)
            {
                return ServiceResult<object>.NotFound("Banner khong ton tai tren he thong.");
            }

            ApplyUpsert(banner, dto);
            Touch(banner);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Cap nhat banner thanh cong.");
        }

        public async Task<ServiceResult<object>> DeleteBannerAsync(long id)
        {
            var banner = await _context.Banners.FirstOrDefaultAsync(b => b.BannerId == id);
            if (banner == null)
            {
                return ServiceResult<object>.NotFound("Banner khong ton tai tren he thong.");
            }

            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Xoa banner thanh cong.");
        }

        public async Task<ServiceResult<object>> UpdateBannerStatusAsync(long id, BannerStatusUpdateDto dto)
        {
            var banner = await _context.Banners.FirstOrDefaultAsync(b => b.BannerId == id);
            if (banner == null)
            {
                return ServiceResult<object>.NotFound("Banner khong ton tai tren he thong.");
            }

            banner.IsActive = dto.IsActive;
            Touch(banner);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Cap nhat trang thai banner thanh cong.");
        }

        private static string? ValidateBanner(BannerUpsertDto dto)
        {
            if (dto.EndDate.HasValue && dto.StartDate.HasValue && dto.EndDate.Value < dto.StartDate.Value)
            {
                return "Ngay ket thuc phai lon hon hoac bang ngay bat dau.";
            }

            if (string.Equals(dto.TargetType.Trim(), "custom", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrWhiteSpace(dto.CustomUrl))
            {
                return "CustomUrl la bat buoc khi TargetType la custom.";
            }

            return null;
        }

        private static void ApplyUpsert(Banner banner, BannerUpsertDto dto)
        {
            banner.Title = dto.Title.Trim();
            banner.ImageUrl = dto.ImageUrl.Trim();
            banner.TargetType = dto.TargetType.Trim();
            banner.TargetId = string.IsNullOrWhiteSpace(dto.TargetId) ? null : dto.TargetId.Trim();
            banner.CustomUrl = string.IsNullOrWhiteSpace(dto.CustomUrl) ? null : dto.CustomUrl.Trim();
            banner.Position = string.IsNullOrWhiteSpace(dto.Position) ? "HOME_TOP" : dto.Position.Trim();
            banner.DisplayOrder = dto.DisplayOrder;
            banner.IsActive = dto.IsActive;
            banner.StartDate = dto.StartDate ?? DateTime.UtcNow;
            banner.EndDate = dto.EndDate;
        }

        private static void Touch(Banner banner)
        {
            banner.UpdatedAt = DateTime.UtcNow;
            banner.Version += 1;
        }

        private static BannerDto MapBanner(Banner banner)
        {
            return new BannerDto
            {
                BannerId = banner.BannerId,
                Title = banner.Title,
                ImageUrl = banner.ImageUrl,
                TargetType = banner.TargetType,
                TargetId = banner.TargetId,
                CustomUrl = banner.CustomUrl,
                Position = banner.Position,
                DisplayOrder = banner.DisplayOrder,
                IsActive = banner.IsActive,
                StartDate = banner.StartDate,
                EndDate = banner.EndDate,
                CreatedAt = banner.CreatedAt,
                UpdatedAt = banner.UpdatedAt,
                Version = banner.Version
            };
        }
    }
}
