
namespace MyAPI.Application.Interfaces.Customer
{
    public interface IReviewService
    {
        Task<ServiceResult<object>> GetProductReviewsAsync(int productId);
        Task<ServiceResult<object>> CreateReviewAsync(int productId, int userId, CreateReviewDto dto);
        Task<ServiceResult<object>> UpdateReviewAsync(int id, int userId, UpdateReviewDto dto);
        Task<ServiceResult<object>> DeleteReviewAsync(int id, int userId, bool isAdmin);
    }
}


