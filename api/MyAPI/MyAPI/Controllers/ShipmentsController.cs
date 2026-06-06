using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.DTOs;
using System.Security.Claims;

namespace MyAPI.Controllers
{
    [Route("api/shipments")]
    [ApiController]
    [Authorize]
    public class ShipmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ShipmentsController(AppDbContext context)
        {
            _context = context;
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

        private static ShipmentDto MapShipment(Shipment shipment)
        {
            return new ShipmentDto
            {
                ShipmentId = shipment.ShipmentId,
                OrderId = shipment.OrderId,
                TrackingNumber = shipment.TrackingNumber,
                Courier = shipment.Courier,
                ShippedDate = shipment.ShippedDate,
                DeliveredDate = shipment.DeliveredDate,
                Status = shipment.Status
            };
        }

        private static bool IsValidStatus(string status)
        {
            var validStatuses = new[] { "preparing", "shipping", "delivered" };
            return validStatuses.Contains(status);
        }

        [HttpGet("order/{orderId:int}")]
        public async Task<IActionResult> GetShipmentByOrder(int orderId)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại." });
            }

            var order = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound(new { message = "Không tìm thấy đơn hàng." });
            }

            if (!IsAdminOrStaff() && order.UserId != userId)
            {
                return Forbid();
            }

            var shipment = await _context.Shipments.AsNoTracking().FirstOrDefaultAsync(s => s.OrderId == orderId);

            if (shipment == null)
            {
                return NotFound(new { message = "Đơn hàng chưa có thông tin vận chuyển." });
            }

            return Ok(MapShipment(shipment));
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost]
        public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentDto dto)
        {
            if (!IsValidStatus(dto.Status))
            {
                return BadRequest(new { message = "Trạng thái vận chuyển không hợp lệ." });
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == dto.OrderId);

            if (order == null)
            {
                return NotFound(new { message = "Không tìm thấy đơn hàng." });
            }

            var exists = await _context.Shipments.AnyAsync(s => s.OrderId == dto.OrderId);

            if (exists)
            {
                return BadRequest(new { message = "Đơn hàng đã có thông tin vận chuyển." });
            }

            var shipment = new Shipment
            {
                OrderId = dto.OrderId,
                TrackingNumber = string.IsNullOrWhiteSpace(dto.TrackingNumber) ? null : dto.TrackingNumber.Trim(),
                Courier = string.IsNullOrWhiteSpace(dto.Courier) ? null : dto.Courier.Trim(),
                ShippedDate = dto.ShippedDate,
                DeliveredDate = dto.DeliveredDate,
                Status = dto.Status
            };

            _context.Shipments.Add(shipment);
            order.TrackingCode = shipment.TrackingNumber;
            order.Status = shipment.Status == "delivered" ? "completed" : shipment.Status;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetShipmentByOrder), new { orderId = shipment.OrderId }, MapShipment(shipment));
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateShipment(int id, [FromBody] UpdateShipmentDto dto)
        {
            if (!IsValidStatus(dto.Status))
            {
                return BadRequest(new { message = "Trạng thái vận chuyển không hợp lệ." });
            }

            var shipment = await _context.Shipments.FirstOrDefaultAsync(s => s.ShipmentId == id);

            if (shipment == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin vận chuyển." });
            }

            shipment.TrackingNumber = string.IsNullOrWhiteSpace(dto.TrackingNumber) ? null : dto.TrackingNumber.Trim();
            shipment.Courier = string.IsNullOrWhiteSpace(dto.Courier) ? null : dto.Courier.Trim();
            shipment.ShippedDate = dto.ShippedDate;
            shipment.DeliveredDate = dto.DeliveredDate;
            shipment.Status = dto.Status;

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == shipment.OrderId);

            if (order != null)
            {
                order.TrackingCode = shipment.TrackingNumber;
                order.Status = shipment.Status == "delivered" ? "completed" : shipment.Status;
                order.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cập nhật vận chuyển thành công.",
                shipment = MapShipment(shipment)
            });
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateShipmentStatus(int id, [FromBody] UpdateShipmentStatusDto dto)
        {
            if (!IsValidStatus(dto.Status))
            {
                return BadRequest(new { message = "Trạng thái vận chuyển không hợp lệ." });
            }

            var shipment = await _context.Shipments.FirstOrDefaultAsync(s => s.ShipmentId == id);

            if (shipment == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin vận chuyển." });
            }

            shipment.Status = dto.Status;

            if (dto.Status == "shipping" && shipment.ShippedDate == null)
            {
                shipment.ShippedDate = DateTime.UtcNow;
            }

            if (dto.Status == "delivered")
            {
                shipment.DeliveredDate ??= DateTime.UtcNow;
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == shipment.OrderId);

            if (order != null)
            {
                order.Status = dto.Status == "delivered" ? "completed" : dto.Status;
                order.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cập nhật trạng thái giao hàng thành công.",
                shipment = MapShipment(shipment)
            });
        }
    }
}
