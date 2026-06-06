using Microsoft.AspNetCore.Authorization;
using MyAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using System.Security.Claims;

namespace MyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
     [Authorize]
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

         [HttpGet]
         [Authorize(Roles = "admin")]
         public async Task<IActionResult> GetUsers()
         {
             var users = await _context.Users.Select(u => new UserDto
             {
                 UserId = u.UserId,
                 Email = u.Email,
                 FullName = u.FullName,
                 Phone = u.Phone,
                 Role = u.Role,
                 IsActive = u.IsActive,
                 CreatedAt = u.CreatedAt
             })
             .ToListAsync();

             return Ok(users);
         }
         [HttpGet("{id:int}")]
         [Authorize(Roles = "admin")]
         public async Task<IActionResult> GetUserById(int id)
         {
             var user = await _context.Users.AsNoTracking().Where(u => u.UserId == id).Select(u => new UserDto
             {
                 UserId = u.UserId,
                 Email = u.Email,
                 FullName = u.FullName,
                 Phone = u.Phone,
                 Role = u.Role,
                 IsActive = u.IsActive,
                 CreatedAt = u.CreatedAt
             }).FirstOrDefaultAsync();

             if (user == null)
             {
                 return NotFound(new { message = "Người dùng không tồn tại" });
             }

             return Ok(user);
         }

         [HttpPut("profile")]
         public async Task<ActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
         {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Không xác định được người dùng" });
            }
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Người dùng không tồn tại" });
            }

            user.FullName = dto.FullName.Trim();
            user.Phone = string.IsNullOrEmpty(dto.Phone) ? null : dto.Phone.Trim();

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thông tin cá nhân thành công" });
         }

         [HttpPut("{id:int}/role")]
         [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
           {
                return NotFound(new { message = "Người dùng không tồn tại" });
            }

            user.Role = dto.Role;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật vai trò người dùng thành công" });
        }
        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateUserStatus(int id, [FromBody] UpdateUserStatusDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Người dùng không tồn tại" });
            }

            user.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return Ok(new { message = dto.IsActive ? "Kích hoạt người dùng thành công" : "Vô hiệu hóa người dùng thành công" });
        }
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Người dùng không tồn tại" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Xóa người dùng thành công" });
        }

    }
}
