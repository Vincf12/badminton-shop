using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyAPI.Controllers.Catalog
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _categoryService.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var result = await _categoryService.GetCategoryAsync(id);
            return ToActionResult(result);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryUpsertDto dto)
        {
            var result = await _categoryService.CreateCategoryAsync(dto);
            if (!result.Succeeded)
            {
                return ToActionResult(result);
            }

            return CreatedAtAction(nameof(GetCategory), new { id = result.Data!.CategoryId }, result.Data);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpsertDto dto)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, dto);
            return ToActionResult(result);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result.Succeeded)
            {
                return ToActionResult(result);
            }

            return NoContent();
        }

        private IActionResult ToActionResult<T>(ServiceResult<T> result)
        {
            if (result.Succeeded)
            {
                return result.Data != null ? Ok(result.Data) : Ok(new { message = result.Message });
            }

            return StatusCode(result.StatusCode, new { message = result.Message });
        }
    }
}


