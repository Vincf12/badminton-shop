using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;

namespace MyAPI.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    [Authorize(Roles = "admin,staff")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
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

            return Ok(new
            {
                totalRevenue,
                pendingRevenue,
                totalOrders,
                totalUsers,
                totalProducts,
                lowStockProducts
            });
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue([FromQuery] string type = "day")
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

                var monthlyRevenue = monthlyRows.Select(x => new
                {
                    period = $"{x.Year:D4}-{x.Month:D2}",
                    x.revenue,
                    x.orderCount
                });

                return Ok(monthlyRevenue);
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

            var dailyRevenue = dailyRows.Select(x => new
            {
                period = $"{x.Year:D4}-{x.Month:D2}-{x.Day:D2}",
                x.revenue,
                x.orderCount
            });

            return Ok(dailyRevenue);
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrdersByStatus()
        {
            var stats = await _context.Orders
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

            return Ok(stats);
        }

        [HttpGet("top-products")]
        public async Task<IActionResult> GetTopProducts([FromQuery] int limit = 10)
        {
            limit = Math.Clamp(limit, 1, 50);

            var products = await (
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

            return Ok(products);
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockProducts([FromQuery] int threshold = 5)
        {
            threshold = Math.Max(0, threshold);

            var products = await (
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

            return Ok(products);
        }

        [HttpGet("new-users")]
        public async Task<IActionResult> GetNewUsers([FromQuery] int limit = 10)
        {
            limit = Math.Clamp(limit, 1, 100);

            var users = await _context.Users
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

            return Ok(users);
        }
    }
}
