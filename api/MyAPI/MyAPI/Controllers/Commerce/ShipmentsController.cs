using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Application.DTOs;
using MyAPI.Services;
using MyAPI.Services.Interfaces;
using System.Security.Claims;

namespace MyAPI.Controllers
{
    [Route("api/shipments")]
    [ApiController]
    [Authorize]
    public class ShipmentsController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;

        public ShipmentsController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out userId);
        }

        private bool IsAdminOrStaff()
        {
            return User.IsInRole("admin") || User.IsInRole("staff");
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

        [HttpGet("order/{orderId:int}")]
        public async Task<IActionResult> GetShipmentByOrder(int orderId)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var result = await _shipmentService.GetShipmentByOrderAsync(orderId, userId, IsAdminOrStaff());
            return ToActionResult(result);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost]
        public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentDto dto)
        {
            var result = await _shipmentService.CreateShipmentAsync(dto);

            if (!result.Succeeded)
            {
                return ToActionResult(result);
            }

            var shipment = (ShipmentDto)result.Data!;
            return CreatedAtAction(nameof(GetShipmentByOrder), new { orderId = shipment.OrderId }, shipment);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateShipment(int id, [FromBody] UpdateShipmentDto dto)
        {
            var result = await _shipmentService.UpdateShipmentAsync(id, dto);
            return ToActionResult(result);
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateShipmentStatus(int id, [FromBody] UpdateShipmentStatusDto dto)
        {
            var result = await _shipmentService.UpdateShipmentStatusAsync(id, dto);
            return ToActionResult(result);
        }
    }
}
