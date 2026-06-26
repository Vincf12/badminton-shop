using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Application.DTOs;
using MyAPI.Services;
using MyAPI.Services.Interfaces;

namespace MyAPI.Controllers
{
    [ApiController]
    public class ProductVariantsController : ControllerBase
    {
        private readonly IProductVariantService _productVariantService;

        public ProductVariantsController(IProductVariantService productVariantService)
        {
            _productVariantService = productVariantService;
        }

        [HttpGet("api/products/{productId:int}/variants")]
        public async Task<IActionResult> GetVariantsByProduct(int productId)
        {
            var variants = await _productVariantService.GetVariantsByProductAsync(productId);
            return Ok(variants);
        }

        [HttpGet("api/product-variants/{id:int}")]
        public async Task<IActionResult> GetVariant(int id)
        {
            var result = await _productVariantService.GetVariantAsync(id);
            return ToActionResult(result);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost("api/products/{productId:int}/variants")]
        public async Task<IActionResult> CreateVariant(int productId, [FromBody] ProductVariantUpsertDto dto)
        {
            var result = await _productVariantService.CreateVariantAsync(productId, dto);
            if (!result.Succeeded)
            {
                return ToActionResult(result);
            }

            var variantId = result.Data?.GetType().GetProperty("variantId")?.GetValue(result.Data);
            return CreatedAtAction(nameof(GetVariant), new { id = variantId }, result.Data);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("api/product-variants/{id:int}")]
        public async Task<IActionResult> UpdateVariant(int id, [FromBody] ProductVariantUpsertDto dto)
        {
            var result = await _productVariantService.UpdateVariantAsync(id, dto);
            return ToActionResult(result);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("api/product-variants/{id:int}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockDto dto)
        {
            var result = await _productVariantService.UpdateStockAsync(id, dto);
            return ToActionResult(result);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpDelete("api/product-variants/{id:int}")]
        public async Task<IActionResult> DeleteVariant(int id)
        {
            var result = await _productVariantService.DeleteVariantAsync(id);
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
