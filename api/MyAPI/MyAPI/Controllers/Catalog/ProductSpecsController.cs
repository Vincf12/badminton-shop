using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Application.DTOs;
using MyAPI.Services;
using MyAPI.Services.Interfaces;

namespace MyAPI.Controllers
{
    [ApiController]
    public class ProductSpecsController : ControllerBase
    {
        private readonly IProductSpecService _productSpecService;

        public ProductSpecsController(IProductSpecService productSpecService)
        {
            _productSpecService = productSpecService;
        }

        [HttpGet("api/products/{productId:int}/specs")]
        public async Task<IActionResult> GetSpecs(int productId)
        {
            var specs = await _productSpecService.GetSpecsAsync(productId);
            return Ok(specs);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost("api/products/{productId:int}/specs")]
        public async Task<IActionResult> CreateSpec(int productId, [FromBody] ProductSpecUpsertDto dto)
        {
            var result = await _productSpecService.CreateSpecAsync(productId, dto);
            return ToActionResult(result);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("api/product-specs/{id:int}")]
        public async Task<IActionResult> UpdateSpec(int id, [FromBody] ProductSpecUpsertDto dto)
        {
            var result = await _productSpecService.UpdateSpecAsync(id, dto);
            return ToActionResult(result);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpDelete("api/product-specs/{id:int}")]
        public async Task<IActionResult> DeleteSpec(int id)
        {
            var result = await _productSpecService.DeleteSpecAsync(id);
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
