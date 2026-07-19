
namespace MyAPI.Application.Interfaces.Store
{
    public interface IBannerService
    {
        Task<IEnumerable<BannerDto>> GetBannersAsync();
        Task<ServiceResult<BannerDto>> GetBannerAsync(long id);
        Task<ServiceResult<BannerDto>> CreateBannerAsync(BannerUpsertDto dto);
        Task<ServiceResult<object>> UpdateBannerAsync(long id, BannerUpsertDto dto);
        Task<ServiceResult<object>> DeleteBannerAsync(long id);
        Task<ServiceResult<object>> UpdateBannerStatusAsync(long id, BannerStatusUpdateDto dto);
    }
}


