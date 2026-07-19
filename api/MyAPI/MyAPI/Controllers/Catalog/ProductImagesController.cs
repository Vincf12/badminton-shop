using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyAPI.Controllers.Catalog
{
    [ApiController]
    public class ProductImagesController : ControllerBase
    {
        private readonly IProductImageService _productImageService;

        public ProductImagesController(IProductImageService productImageService)
        {
            _productImageService = productImageService;
        }

        [HttpGet("api/products/{productId:int}/images")]
        public async Task<IActionResult> GetImages(int productId)
        {
            var images = await _productImageService.GetImagesAsync(productId);
            return Ok(images);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost("api/products/{productId:int}/images")]
        public async Task<IActionResult> CreateImage(int productId, [FromBody] ProductImageUpsertDto dto)
        {
            var result = await _productImageService.CreateImageAsync(productId, dto);
            return ToActionResult(result);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("api/product-images/{id:int}")]
        public async Task<IActionResult> UpdateImage(int id, [FromBody] ProductImageUpsertDto dto)
        {
            var result = await _productImageService.UpdateImageAsync(id, dto);
            return ToActionResult(result);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("api/product-images/{id:int}/main")]
        public async Task<IActionResult> SetMainImage(int id)
        {
            var result = await _productImageService.SetMainImageAsync(id);
            return ToActionResult(result);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpDelete("api/product-images/{id:int}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var result = await _productImageService.DeleteImageAsync(id);
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


