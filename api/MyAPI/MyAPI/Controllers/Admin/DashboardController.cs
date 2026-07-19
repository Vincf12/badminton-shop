using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyAPI.Controllers.Admin
{
    [Route("api/dashboard")]
    [ApiController]
    [Authorize(Roles = "admin,staff")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            return Ok(await _dashboardService.GetOverviewAsync());
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue([FromQuery] string type = "day")
        {
            return Ok(await _dashboardService.GetRevenueAsync(type));
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrdersByStatus()
        {
            return Ok(await _dashboardService.GetOrdersByStatusAsync());
        }

        [HttpGet("top-products")]
        public async Task<IActionResult> GetTopProducts([FromQuery] int limit = 10)
        {
            return Ok(await _dashboardService.GetTopProductsAsync(limit));
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockProducts([FromQuery] int threshold = 5)
        {
            return Ok(await _dashboardService.GetLowStockProductsAsync(threshold));
        }

        [HttpGet("new-users")]
        public async Task<IActionResult> GetNewUsers([FromQuery] int limit = 10)
        {
            return Ok(await _dashboardService.GetNewUsersAsync(limit));
        }
    }
}


