
namespace MyAPI.Application.Interfaces.Store
{
    public interface IStoreService
    {
        /// <summary>
        /// Lấy danh sách cửa hàng (có lọc, tìm kiếm, phân trang).
        /// </summary>
        Task<PagedResult<StoreResponse>> GetAllAsync(StoreFilterRequest request);

        /// <summary>
        /// Lấy thông tin chi tiết cửa hàng.
        /// </summary>
        Task<StoreDetailResponse?> GetByIdAsync(int storeId);

        /// <summary>
        /// Lấy danh sách cửa hàng cho dropdown.
        /// </summary>
        Task<IEnumerable<StoreSummaryResponse>> GetLookupAsync();

        /// <summary>
        /// Tạo cửa hàng mới.
        /// </summary>
        Task<StoreResponse> CreateAsync(CreateStoreRequest request);

        /// <summary>
        /// Cập nhật thông tin cửa hàng.
        /// </summary>
        Task<bool> UpdateAsync(int storeId, UpdateStoreRequest request);

        /// <summary>
        /// Bật/Tắt trạng thái hoạt động.
        /// </summary>
        Task<bool> ChangeStatusAsync(int storeId, bool isActive);

        /// <summary>
        /// Xóa cửa hàng.
        /// </summary>
        Task<bool> DeleteAsync(int storeId);
    }
}

