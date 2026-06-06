using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MyAPI.Controllers
{
    [Route("api/addresses")]
    [ApiController]
    [Authorize]
    public class AddressController : Controller
    {
        private readonly AppDbContext _context;

        public AddressController(AppDbContext context)
        {
            _context = context;
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out userId);
        }

        [HttpGet]
        public async Task<IActionResult> GetAddresses()
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại" });
            }
            var addresses = await _context.Addresses.AsNoTracking().Where(a => a.UserId == userId).OrderByDescending(a => a.IsDefault).ThenByDescending(a => a.AddressId)
                .Select(a => new AddressDto
                {
                    AddressId = a.AddressId,
                    RecipientName = a.RecipientName,
                    Phone = a.Phone,
                    Province = a.Province,
                    Ward = a.Ward,
                    District = a.District,
                    AddressDetail = a.AddressDetail,
                    IsDefault = a.IsDefault
                })
                .ToListAsync();
            return Ok(addresses);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] AddressUpsertDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại" });
            }
            if (dto.IsDefault)
            {
                var oldDefaults = await _context.Addresses.Where(a => a.UserId == userId && a.IsDefault).ToListAsync();
                for (int i = 0; i < oldDefaults.Count; i++)
                {
                    oldDefaults[i].IsDefault = false;
                }
            }
            var address = new Address
            {
                UserId = userId,
                RecipientName = dto.RecipientName.Trim(),
                Phone = dto.Phone.Trim(),
                AddressDetail = dto.AddressDetail.Trim(),
                Ward = string.IsNullOrWhiteSpace(dto.Ward) ? null : dto.Ward.Trim(),
                District = string.IsNullOrWhiteSpace(dto.District) ? null : dto.District.Trim(),
                Province = string.IsNullOrWhiteSpace(dto.Province) ? null : dto.Province.Trim(),
                IsDefault = dto.IsDefault
            };
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Thêm địa chỉ thành công", addressId = address.AddressId });
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressUpsertDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại" });
            }
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == id && a.UserId == userId);
            if (address == null)
            {
                return NotFound(new { message = "Địa chỉ không tồn tại" });
            }
            if (dto.IsDefault)
            {
                var oldDefaults = await _context.Addresses.Where(a => a.UserId == userId && a.IsDefault).ToListAsync();
                for (int i = 0; i < oldDefaults.Count; i++)
                {
                    oldDefaults[i].IsDefault = false;
                }
            }
            address.RecipientName = dto.RecipientName.Trim();
            address.Phone = dto.Phone.Trim();
            address.AddressDetail = dto.AddressDetail.Trim();
            address.Ward = string.IsNullOrWhiteSpace(dto.Ward) ? null : dto.Ward.Trim();
            address.District = string.IsNullOrWhiteSpace(dto.District) ? null : dto.District.Trim();
            address.Province = string.IsNullOrWhiteSpace(dto.Province) ? null : dto.Province.Trim();
            address.IsDefault = dto.IsDefault;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật địa chỉ thành công" });
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại" });
            }
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == id && a.UserId == userId);
            if (address == null)
            {
                return NotFound(new { message = "Địa chỉ không tồn tại" });
            }
            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Xóa địa chỉ thành công" });
        }
        [HttpPut("{id:int}/default")]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            if (!TryGetCurrentUserId(out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng hiện tại" });
            }
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == id && a.UserId == userId);
            if (address == null)
            {
                return NotFound(new { message = "Địa chỉ không tồn tại" });
            }
            var oldDefaults = await _context.Addresses.Where(a => a.UserId == userId && a.IsDefault).ToListAsync();
            for (int i = 0; i < oldDefaults.Count; i++)
            {
                oldDefaults[i].IsDefault = false;
            }
            address.IsDefault = true;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đặt địa chỉ mặc định thành công" });
        }
    }
}
