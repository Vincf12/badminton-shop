using MyAPI.Application.DTOs;

namespace MyAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
        Task<ServiceResult<CategoryDto>> GetCategoryAsync(int id);
        Task<ServiceResult<CategoryDto>> CreateCategoryAsync(CategoryUpsertDto dto);
        Task<ServiceResult<object>> UpdateCategoryAsync(int id, CategoryUpsertDto dto);
        Task<ServiceResult<object>> DeleteCategoryAsync(int id);
    }
}
