namespace MyAPI.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<object> GetOverviewAsync();
        Task<object> GetRevenueAsync(string type);
        Task<object> GetOrdersByStatusAsync();
        Task<object> GetTopProductsAsync(int limit);
        Task<object> GetLowStockProductsAsync(int threshold);
        Task<object> GetNewUsersAsync(int limit);
    }
}
