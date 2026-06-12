using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Models.DTOs;
using MyAPI.Services;
using MyAPI.Services.Interfaces;

namespace MyAPI.Controllers
{
    [Route("api/brands")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetBrands()
        {
            var brands = await _brandService.GetBrandsAsync();
            return Ok(brands);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBrand(int id)
        {
            var result = await _brandService.GetBrandAsync(id);
            return ToActionResult(result);
        }

        [HttpPost]
        [Authorize(Roles = "admin,staff")]
        public async Task<IActionResult> CreateBrand([FromBody] BrandUpsertDto dto)
        {
            var result = await _brandService.CreateBrandAsync(dto);
            if (!result.Succeeded)
            {
                return ToActionResult(result);
            }

            return CreatedAtAction(nameof(GetBrand), new { id = result.Data!.BrandId }, result.Data);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin,staff")]
        public async Task<IActionResult> UpdateBrand(int id, [FromBody] BrandUpsertDto dto)
        {
            var result = await _brandService.UpdateBrandAsync(id, dto);
            return ToActionResult(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var result = await _brandService.DeleteBrandAsync(id);
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
