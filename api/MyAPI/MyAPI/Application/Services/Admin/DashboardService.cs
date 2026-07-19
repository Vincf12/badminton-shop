using Microsoft.EntityFrameworkCore;

namespace MyAPI.Application.Services.Admin
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> GetOverviewAsync()
        {
            var totalRevenue = await _context.Orders
                .AsNoTracking()
                .Where(o => o.Status == "completed")
                .SumAsync(o => (decimal?)o.FinalAmount) ?? 0;

            var pendingRevenue = await _context.Orders
                .AsNoTracking()
                .Where(o => o.Status != "cancelled")
                .SumAsync(o => (decimal?)o.FinalAmount) ?? 0;

            var totalOrders = await _context.Orders.CountAsync();
            var totalUsers = await _context.Users.CountAsync();
            var totalProducts = await _context.Products.CountAsync();
            var lowStockProducts = await _context.ProductVariants.CountAsync(v => v.StockQuantity <= 5);

            return new
            {
                totalRevenue,
                pendingRevenue,
                totalOrders,
                totalUsers,
                totalProducts,
                lowStockProducts
            };
        }

        public async Task<object> GetRevenueAsync(string type)
        {
            type = type.Trim().ToLower();

            if (type == "month")
            {
                var monthlyRows = await _context.Orders
                    .AsNoTracking()
                    .Where(o => o.Status == "completed")
                    .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        revenue = g.Sum(o => o.FinalAmount),
                        orderCount = g.Count()
                    })
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Month)
                    .ToListAsync();

                return monthlyRows.Select(x => new
                {
                    period = $"{x.Year:D4}-{x.Month:D2}",
                    x.revenue,
                    x.orderCount
                });
            }

            var dailyRows = await _context.Orders
                .AsNoTracking()
                .Where(o => o.Status == "completed")
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month, o.CreatedAt.Day })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    g.Key.Day,
                    revenue = g.Sum(o => o.FinalAmount),
                    orderCount = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ThenBy(x => x.Day)
                .ToListAsync();

            return dailyRows.Select(x => new
            {
                period = $"{x.Year:D4}-{x.Month:D2}-{x.Day:D2}",
                x.revenue,
                x.orderCount
            });
        }

        public async Task<object> GetOrdersByStatusAsync()
        {
            return await _context.Orders
                .AsNoTracking()
                .GroupBy(o => o.Status)
                .Select(g => new
                {
                    status = g.Key,
                    count = g.Count(),
                    totalAmount = g.Sum(o => o.FinalAmount)
                })
                .OrderBy(x => x.status)
                .ToListAsync();
        }

        public async Task<object> GetTopProductsAsync(int limit)
        {
            limit = Math.Clamp(limit, 1, 50);

            return await (
                from detail in _context.OrderDetails.AsNoTracking()
                join variant in _context.ProductVariants.AsNoTracking()
                    on detail.VariantId equals variant.VariantId
                join product in _context.Products.AsNoTracking()
                    on variant.ProductId equals product.ProductId
                group new { detail, product } by new
                {
                    product.ProductId,
                    product.ProductName
                }
                into g
                orderby g.Sum(x => x.detail.Quantity) descending
                select new
                {
                    productId = g.Key.ProductId,
                    productName = g.Key.ProductName,
                    soldQuantity = g.Sum(x => x.detail.Quantity),
                    revenue = g.Sum(x => x.detail.SubTotal)
                })
                .Take(limit)
                .ToListAsync();
        }

        public async Task<object> GetLowStockProductsAsync(int threshold)
        {
            threshold = Math.Max(0, threshold);

            return await (
                from variant in _context.ProductVariants.AsNoTracking()
                join product in _context.Products.AsNoTracking()
                    on variant.ProductId equals product.ProductId
                where variant.StockQuantity <= threshold
                orderby variant.StockQuantity, product.ProductName
                select new
                {
                    productId = product.ProductId,
                    productName = product.ProductName,
                    variantId = variant.VariantId,
                    sku = variant.Sku,
                    weight = variant.Weight,
                    gripSize = variant.GripSize,
                    color = variant.Color,
                    stockQuantity = variant.StockQuantity
                })
                .ToListAsync();
        }

        public async Task<object> GetNewUsersAsync(int limit)
        {
            limit = Math.Clamp(limit, 1, 100);

            return await _context.Users
                .AsNoTracking()
                .OrderByDescending(u => u.CreatedAt)
                .Take(limit)
                .Select(u => new
                {
                    u.UserId,
                    u.Email,
                    u.FullName,
                    u.Phone,
                    u.Role,
                    u.IsActive,
                    u.CreatedAt
                })
                .ToListAsync();
        }
    }
}


