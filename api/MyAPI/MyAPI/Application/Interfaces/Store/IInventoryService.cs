
namespace MyAPI.Application.Interfaces.Store
{
    public interface IInventoryService
    {
        /// <summary>
        /// Lấy danh sách tồn kho của cửa hàng với các bộ lọc và phân trang.
        /// </summary>
        Task<PagedResult<InventoryResponse>> GetAllAsync(InventoryFilterRequest request);

        /// <summary>
        /// Lấy chi tiết tồn kho của một sản phẩm cụ thể trong cửa hàng.
        /// </summary>
        Task<InventoryDetailResponse?> GetByIdAsync(int storeId, int variantId);

        /// <summary>
        /// Lấy danh sách sản phẩm có số lượng tồn kho thấp hơn ngưỡng cảnh báo.
        /// </summary>
        Task<PagedResult<LowStockResponse>> GetLowStockAsync(int storeId);

        /// <summary>
        /// lập hồ sơ theo dõi chính xác số lượng hàng hóa, nguyên vật liệu hiện có.
        /// </summary>
        Task<InventoryResponse> CreateAsync(int storeId, CreateInventoryRequest request);

        /// <summary>
        /// Cập nhật số lượng tồn kho và ngưỡng cảnh báo.
        /// </summary>
        Task<bool> UpdateAsync(int storeId, int variantId, UpdateInventoryRequest request);

        /// <summary>
        /// Điều chỉnh số lượng nhập/xuất kho.
        /// </summary>
        Task<bool> AdjustAsync(int storeId, AdjustInventoryRequest request);

        /// <summary>
        /// Xóa bản ghi tồn kho.
        /// </summary>
        Task<bool> DeleteAsync(int storeId, int variantId);
    }
}

