using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyAPI.Controllers.Store
{
    [Route("api/stores")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoresController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<StoreResponse>>> GetStores([FromQuery] StoreFilterRequest request)
        {
            var stores = await _storeService.GetAllAsync(request);
            return Ok(stores);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetStore(long id)
        {
            var result = await _storeService.GetByIdAsync((int)id);
            if (result == null)
            {
                return NotFound(new { message = "KhÃ´ng tÃ¬m tháº¥y cá»­a hÃ ng." });
            }

            return Ok(result);
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> GetLookup()
        {
            var stores = await _storeService.GetLookupAsync();
            return Ok(stores);
        }

        [HttpPost]
        [Authorize(Roles = "admin,staff")]
        public async Task<IActionResult> CreateStore([FromBody] CreateStoreRequest request)
        {
            var result = await _storeService.CreateAsync(request);
            return CreatedAtAction(nameof(GetStore), new { id = result.StoreId }, result);
        }

        [HttpPut("{id:long}")]
        [Authorize(Roles = "admin,staff")]
        public async Task<IActionResult> UpdateStore(long id, [FromBody] UpdateStoreRequest request)
        {
            var updated = await _storeService.UpdateAsync((int)id, request);

            if (!updated)
            {
                return NotFound(new { message = "KhÃ´ng tÃ¬m tháº¥y cá»­a hÃ ng." });
            }

            return NoContent();
        }

        [HttpPatch("{id:long}/status")]
        [Authorize(Roles = "admin,staff")]
        public async Task<IActionResult> ChangeStatus(long id, [FromQuery] bool isActive)
        {
            var updated = await _storeService.ChangeStatusAsync((int)id, isActive);

            if (!updated)
            {
                return NotFound(new { message = "KhÃ´ng tÃ¬m tháº¥y cá»­a hÃ ng." });
            }

            return NoContent();
        }

        [HttpDelete("{id:long}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteStore(long id)
        {
            var deleted = await _storeService.DeleteAsync((int)id);

            if (!deleted)
            {
                return NotFound(new { message = "KhÃ´ng tÃ¬m tháº¥y cá»­a hÃ ng." });
            }

            return NoContent();
        }
    }
}
