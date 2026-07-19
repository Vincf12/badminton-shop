using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyAPI.Controllers.Store
{
    [ApiController]
    [Route("api/stores/{storeId:int}/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        /// <summary>
        /// Láº¥y danh sÃ¡ch tá»“n kho cá»§a cá»­a hÃ ng
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            int storeId,
            [FromQuery] InventoryFilterRequest request)
        {
            request.StoreId = storeId;

            var result = await _inventoryService.GetAllAsync(request);

            return Ok(result);
        }

        /// <summary>
        /// Láº¥y chi tiáº¿t tá»“n kho theo Variant
        /// </summary>
        [HttpGet("{variantId:int}")]
        public async Task<IActionResult> GetById(
            int storeId,
            int variantId)
        {
            var result = await _inventoryService.GetByIdAsync(storeId, variantId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// ThÃªm Variant vÃ o kho cá»­a hÃ ng
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(
            int storeId,
            [FromBody] CreateInventoryRequest request)
        {
            await _inventoryService.CreateAsync(storeId, request);

            return Ok(new
            {
                message = "ThÃªm tá»“n kho thÃ nh cÃ´ng."
            });
        }

        /// <summary>
        /// Cáº­p nháº­t tá»“n kho
        /// </summary>
        [HttpPut("{variantId:int}")]
        public async Task<IActionResult> Update(
            int storeId,
            int variantId,
            [FromBody] UpdateInventoryRequest request)
        {
            var success = await _inventoryService.UpdateAsync(storeId, variantId, request);

            if (!success)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Äiá»u chá»‰nh tá»“n kho
        /// </summary>
        [HttpPost("adjust")]
        public async Task<IActionResult> Adjust(
            int storeId,
            [FromBody] AdjustInventoryRequest request)
        {
            await _inventoryService.AdjustAsync(storeId, request);

            return Ok(new
            {
                message = "Äiá»u chá»‰nh tá»“n kho thÃ nh cÃ´ng."
            });
        }

        /// <summary>
        /// Danh sÃ¡ch sáº£n pháº©m sáº¯p háº¿t hÃ ng
        /// </summary>
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock(int storeId)
        {
            var result = await _inventoryService.GetLowStockAsync(storeId);

            return Ok(result);
        }

        /// <summary>
        /// XÃ³a Variant khá»i kho
        /// </summary>
        [HttpDelete("{variantId:int}")]
        public async Task<IActionResult> Delete(
            int storeId,
            int variantId)
        {
            var success = await _inventoryService.DeleteAsync(storeId, variantId);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
