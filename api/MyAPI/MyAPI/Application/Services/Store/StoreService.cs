using Microsoft.EntityFrameworkCore;
using StoreEntity = MyAPI.Domain.Entities.Store.Store;

namespace MyAPI.Application.Services.Store
{
    public class StoreService : IStoreService
    {
        private readonly AppDbContext _context;

        public StoreService(AppDbContext context)
        {
            _context = context;
        }
    // Lấy danh sách cửa hàng với phân trang và lọc
    public async Task<PagedResult<StoreResponse>> GetAllAsync(StoreFilterRequest request)
    {
        var query = _context.Stores
        .AsNoTracking()
        .AsQueryable();
        
        // Tìm kiếm theo tên hoặc mã cửa hàng
        if (!string.IsNullOrEmpty(request.Keyword))
        {
            query = query.Where(s => s.StoreName.Contains(request.Keyword) || s.StoreCode.Contains(request.Keyword));
        }

        // Lọc theo tỉnh/thành phố
        if (!string.IsNullOrEmpty(request.Province))
        {
            query = query.Where(s => s.Province == request.Province);
        }

        // Lọc theo trạng thái hoạt động
        if (request.IsActive.HasValue)
        {
            query = query.Where(s => s.IsActive == request.IsActive.Value);
        }

        // Lọc theo điểm lấy hàng
        if (request.IsPickupPoint.HasValue)
        {
            query = query.Where(s => s.IsPickupPoint == request.IsPickupPoint.Value);
        }

        // Tổng số bản ghi

        var totalRecords = await query.CountAsync();

        // PhÃ¢n trang
        var stores = await query
            .OrderBy(s => s.StoreName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new StoreResponse
            {
                StoreId = s.StoreId,
                StoreCode = s.StoreCode,
                StoreName = s.StoreName,
                Phone = s.Phone,
                Province = s.Province,
                Ward = s.Ward,
                AddressDetail = s.AddressDetail,
                IsActive = s.IsActive,
                IsPickupPoint = s.IsPickupPoint,
                OpeningTime = s.OpeningTime,
                ClosingTime = s.ClosingTime
            })
            .ToListAsync();

        return new PagedResult<StoreResponse>
        {
            Items = stores,
            TotalItems = totalRecords,
            Page = request.Page,
            PageSize = request.PageSize
        };

    }

    // Lấy thông tin chi tiết cửa hàng theo ID
    public async Task<StoreDetailResponse?> GetByIdAsync(int storeId)
    {
        // Kiểm tra xem cửa hàng có tồn tại không
        var store = await _context.Stores
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.StoreId == storeId);
            if (store == null)
            {
                return null;
            }

            return new StoreDetailResponse
            {
                StoreId = store.StoreId,
                StoreCode = store.StoreCode,
                StoreName = store.StoreName,
                Phone = store.Phone,
                Province = store.Province,
                Ward = store.Ward,
                AddressDetail = store.AddressDetail,
                IsActive = store.IsActive,
                IsPickupPoint = store.IsPickupPoint,
                OpeningTime = store.OpeningTime,
                ClosingTime = store.ClosingTime
            };
    }

    // Lấy danh sách cửa hàng cho dropdown
    public async Task<IEnumerable<StoreSummaryResponse>> GetLookupAsync()
    {
        return await _context.Stores
            .AsNoTracking()
            .Where(s => s.IsActive) // Chỉ lấy các cửa hàng đang hoạt động
            .Select(s => new StoreSummaryResponse
            {
                StoreId = s.StoreId,
                StoreName = s.StoreName,
                StoreCode = s.StoreCode,
                Ward = s.Ward,
                Province = s.Province,
            })
            .ToListAsync();
    }

    public async Task<StoreResponse> CreateAsync(CreateStoreRequest request)
    {
        // Kiểm tra mã cửa hàng đã tồn tại
        var existingStore = await _context.Stores
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.StoreCode == request.StoreCode);

        if (existingStore != null)
        {
            throw new InvalidOperationException("Mã cửa hàng đã tồn tại.");
        }

        var store = new StoreEntity
        {
            StoreCode = request.StoreCode,
            StoreName = request.StoreName,
            Phone = request.Phone,
            Province = request.Province,
            Ward = request.Ward,
            AddressDetail = request.AddressDetail,
            IsActive = request.IsActive,
            IsPickupPoint = request.IsPickupPoint,
            OpeningTime = request.OpeningTime,
            ClosingTime = request.ClosingTime,
        };

        _context.Stores.Add(store);
        await _context.SaveChangesAsync();

        return new StoreResponse
        {
            StoreId = store.StoreId,
            StoreCode = store.StoreCode,
            StoreName = store.StoreName,
            Phone = store.Phone,
            Province = store.Province,
            Ward = store.Ward,
            AddressDetail = store.AddressDetail,
            IsActive = store.IsActive,
            IsPickupPoint = store.IsPickupPoint,
            OpeningTime = store.OpeningTime,
            ClosingTime = store.ClosingTime
        };

    }

    public async Task<bool> UpdateAsync(int storeId, UpdateStoreRequest request)
    {
        // code
        return true; 
    }

    public async Task<bool> ChangeStatusAsync(int storeId, bool isActive)
    {
        // code
        return true;
    }

    public async Task<bool> DeleteAsync(int storeId)
    {
        // code
        return true;
    }
    }
}

