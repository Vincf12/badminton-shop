using Microsoft.EntityFrameworkCore;
using StoreEntity = MyAPI.Domain.Entities.Store;

namespace MyAPI.Application.Services.Store
{
    public class StoreService : IStoreService
    {
        private readonly AppDbContext _context;

        public StoreService(AppDbContext context)
        {
            _context = context;
        }
    //Láº¥y danh sÃ¡ch cá»­a hÃ ng vá»›i phÃ¢n trang vÃ  lá»c
    public async Task<PagedResult<StoreResponse>> GetAllAsync(StoreFilterRequest request)
    {
        var query = _context.Stores
        .AsNoTracking()
        .AsQueryable();
        
        //  // TÃ¬m kiáº¿m theo tÃªn hoáº·c mÃ£ cá»­a hÃ ng
        if (!string.IsNullOrEmpty(request.Keyword))
        {
            query = query.Where(s => s.StoreName.Contains(request.Keyword) || s.StoreCode.Contains(request.Keyword));
        }

        // Lá»c theo tá»‰nh/thÃ nh phá»‘
        if (!string.IsNullOrEmpty(request.Province))
        {
            query = query.Where(s => s.Province == request.Province);
        }

        // Lá»c theo tráº¡ng thÃ¡i hoáº¡t Ä‘á»™ng
        if (request.IsActive.HasValue)
        {
            query = query.Where(s => s.IsActive == request.IsActive.Value);
        }

        // Lá»c theo Ä‘iá»ƒm láº¥y hÃ ng
        if (request.IsPickupPoint.HasValue)
        {
            query = query.Where(s => s.IsPickupPoint == request.IsPickupPoint.Value);
        }

        // Tá»•ng sá»‘ báº£n ghi

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

    //Láº¥y chi tiáº¿t cá»­a hÃ ng theo ID
    public async Task<StoreDetailResponse?> GetByIdAsync(int storeId)
    {
        // Kiá»ƒm tra xem cá»­a hÃ ng cÃ³ tá»“n táº¡i khÃ´ng
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

    // Láº¥y danh sÃ¡ch cá»­a hÃ ng cho dropdown
    public async Task<IEnumerable<StoreSummaryResponse>> GetLookupAsync()
    {
        return await _context.Stores
            .AsNoTracking()
            .Where(s => s.IsActive) // Chá»‰ láº¥y cÃ¡c cá»­a hÃ ng Ä‘ang hoáº¡t Ä‘á»™ng
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
        // Kiá»ƒm tra mÃ£ cá»­a hÃ ng Ä‘Ã£ tá»“n táº¡i
        var existingStore = await _context.Stores
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.StoreCode == request.StoreCode);

        if (existingStore != null)
        {
            throw new InvalidOperationException("MÃ£ cá»­a hÃ ng Ä‘Ã£ tá»“n táº¡i.");
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

