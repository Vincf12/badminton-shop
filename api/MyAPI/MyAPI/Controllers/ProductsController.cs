using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Models.DTOs;
using MyAPI.Services;
using MyAPI.Services.Interfaces;

namespace MyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        private IActionResult ToActionResult<T>(ServiceResult<T> result)
        {
            if (result.Succeeded)
            {
                return Ok(result.Data);
            }

            if (result.StatusCode == StatusCodes.Status403Forbidden)
            {
                return Forbid();
            }

            return StatusCode(result.StatusCode, new { message = result.Message });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductListItemDto>>> GetProducts(
            [FromQuery] string? search,
            [FromQuery] int? categoryId)
        {
            return Ok(await _productService.GetProductsAsync(search, categoryId));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var result = await _productService.GetProductAsync(id);
            return ToActionResult(result);
        }

        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetProductBySlug(string slug)
        {
            var result = await _productService.GetProductBySlugAsync(slug);
            return ToActionResult(result);
        }

        [HttpPost]
        [Authorize(Roles = "admin,staff")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductUpsertDto dto)
        {
            var result = await _productService.CreateProductAsync(dto);

            if (!result.Succeeded)
            {
                return ToActionResult(result);
            }

            return CreatedAtAction(nameof(GetProduct), new { id = result.Data!.ProductId }, result.Data);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin,staff")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpsertDto dto)
        {
            var result = await _productService.UpdateProductAsync(id, dto);
            return ToActionResult(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            return ToActionResult(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string keyword)
        {
            var result = await _productService.SearchProductsAsync(keyword);
            return ToActionResult(result);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterProducts(
            [FromQuery] int? categoryId,
            [FromQuery] int? brandId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            var result = await _productService.FilterProductsAsync(categoryId, brandId, minPrice, maxPrice);
            return ToActionResult(result);
        }
    }
}
