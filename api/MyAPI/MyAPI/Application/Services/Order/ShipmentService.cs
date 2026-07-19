using Microsoft.EntityFrameworkCore;

namespace MyAPI.Application.Services.Order
{
    public class ShipmentService : IShipmentService
    {
        private readonly AppDbContext _context;

        public ShipmentService(AppDbContext context)
        {
            _context = context;
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

        private static string MapOrderStatus(string shipmentStatus)
        {
            return shipmentStatus == "delivered" ? "completed" : shipmentStatus;
        }

        public async Task<ServiceResult<object>> GetShipmentByOrderAsync(int orderId, int currentUserId, bool isAdminOrStaff)
        {
            var order = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return ServiceResult<object>.NotFound("KhÃ´ng tÃ¬m tháº¥y Ä‘Æ¡n hÃ ng.");
            }

            if (!isAdminOrStaff && order.UserId != currentUserId)
            {
                return ServiceResult<object>.Forbidden();
            }

            var shipment = await _context.Shipments.AsNoTracking().FirstOrDefaultAsync(s => s.OrderId == orderId);

            if (shipment == null)
            {
                return ServiceResult<object>.NotFound("ÄÆ¡n hÃ ng chÆ°a cÃ³ thÃ´ng tin váº­n chuyá»ƒn.");
            }

            return ServiceResult<object>.Ok(MapShipment(shipment));
        }

        public async Task<ServiceResult<object>> CreateShipmentAsync(CreateShipmentDto dto)
        {
            if (!IsValidStatus(dto.Status))
            {
                return ServiceResult<object>.BadRequest("Tráº¡ng thÃ¡i váº­n chuyá»ƒn khÃ´ng há»£p lá»‡.");
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == dto.OrderId);

            if (order == null)
            {
                return ServiceResult<object>.NotFound("KhÃ´ng tÃ¬m tháº¥y Ä‘Æ¡n hÃ ng.");
            }

            var exists = await _context.Shipments.AnyAsync(s => s.OrderId == dto.OrderId);

            if (exists)
            {
                return ServiceResult<object>.BadRequest("ÄÆ¡n hÃ ng Ä‘Ã£ cÃ³ thÃ´ng tin váº­n chuyá»ƒn.");
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
            order.Status = MapOrderStatus(shipment.Status);
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(MapShipment(shipment));
        }

        public async Task<ServiceResult<object>> UpdateShipmentAsync(int id, UpdateShipmentDto dto)
        {
            if (!IsValidStatus(dto.Status))
            {
                return ServiceResult<object>.BadRequest("Tráº¡ng thÃ¡i váº­n chuyá»ƒn khÃ´ng há»£p lá»‡.");
            }

            var shipment = await _context.Shipments.FirstOrDefaultAsync(s => s.ShipmentId == id);

            if (shipment == null)
            {
                return ServiceResult<object>.NotFound("KhÃ´ng tÃ¬m tháº¥y thÃ´ng tin váº­n chuyá»ƒn.");
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
                order.Status = MapOrderStatus(shipment.Status);
                order.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new
            {
                message = "Cáº­p nháº­t váº­n chuyá»ƒn thÃ nh cÃ´ng.",
                shipment = MapShipment(shipment)
            });
        }

        public async Task<ServiceResult<object>> UpdateShipmentStatusAsync(int id, UpdateShipmentStatusDto dto)
        {
            if (!IsValidStatus(dto.Status))
            {
                return ServiceResult<object>.BadRequest("Tráº¡ng thÃ¡i váº­n chuyá»ƒn khÃ´ng há»£p lá»‡.");
            }

            var shipment = await _context.Shipments.FirstOrDefaultAsync(s => s.ShipmentId == id);

            if (shipment == null)
            {
                return ServiceResult<object>.NotFound("KhÃ´ng tÃ¬m tháº¥y thÃ´ng tin váº­n chuyá»ƒn.");
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
                order.Status = MapOrderStatus(dto.Status);
                order.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new
            {
                message = "Cáº­p nháº­t tráº¡ng thÃ¡i giao hÃ ng thÃ nh cÃ´ng.",
                shipment = MapShipment(shipment)
            });
        }
    }
}


