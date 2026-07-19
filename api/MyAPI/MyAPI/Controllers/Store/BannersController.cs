using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyAPI.Controllers.Store
{
    [Route("api/banners")]
    [ApiController]
    public class BannersController : ControllerBase
    {
        private readonly IBannerService _bannerService;

        public BannersController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BannerDto>>> GetBanners()
        {
            var banners = await _bannerService.GetBannersAsync();
            return Ok(banners);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetBanner(long id)
        {
            var result = await _bannerService.GetBannerAsync(id);
            return ToActionResult(result);
        }

        [HttpPost]
        [Authorize(Roles = "admin,staff")]
        public async Task<IActionResult> CreateBanner([FromBody] BannerUpsertDto dto)
        {
            var result = await _bannerService.CreateBannerAsync(dto);
            if (!result.Succeeded)
            {
                return ToActionResult(result);
            }

            return CreatedAtAction(nameof(GetBanner), new { id = result.Data!.BannerId }, result.Data);
        }

        [HttpPut("{id:long}")]
        [Authorize(Roles = "admin,staff")]
        public async Task<IActionResult> UpdateBanner(long id, [FromBody] BannerUpsertDto dto)
        {
            var result = await _bannerService.UpdateBannerAsync(id, dto);
            return ToActionResult(result);
        }

        [HttpDelete("{id:long}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteBanner(long id)
        {
            var result = await _bannerService.DeleteBannerAsync(id);
            if (!result.Succeeded)
            {
                return ToActionResult(result);
            }

            return NoContent();
        }

        [HttpPut("{id:long}/status")]
        [Authorize(Roles = "admin,staff")]
        public async Task<IActionResult> UpdateBannerStatus(long id, [FromBody] BannerStatusUpdateDto dto)
        {
            var result = await _bannerService.UpdateBannerStatusAsync(id, dto);
            return ToActionResult(result);
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


