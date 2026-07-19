using Microsoft.EntityFrameworkCore;

namespace MyAPI.Application.Services.Admin
{
    public class UserAdminService : IUserAdminService
    {
        private readonly AppDbContext _context;

        public UserAdminService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Select(u => MapUser(u))
                .ToListAsync();
        }

        public async Task<ServiceResult<UserDto>> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.UserId == id)
                .Select(u => MapUser(u))
                .FirstOrDefaultAsync();

            return user == null
                ? ServiceResult<UserDto>.NotFound("Nguoi dung khong ton tai.")
                : ServiceResult<UserDto>.Ok(user);
        }

        public async Task<ServiceResult<object>> UpdateUserRoleAsync(int id, UpdateUserRoleDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return ServiceResult<object>.NotFound("Nguoi dung khong ton tai.");
            }

            user.Role = dto.Role;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Cap nhat vai tro nguoi dung thanh cong.");
        }

        public async Task<ServiceResult<object>> UpdateUserStatusAsync(int id, UpdateUserStatusDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return ServiceResult<object>.NotFound("Nguoi dung khong ton tai.");
            }

            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK(dto.IsActive
                ? "Kich hoat nguoi dung thanh cong."
                : "Vo hieu hoa nguoi dung thanh cong.");
        }

        public async Task<ServiceResult<object>> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return ServiceResult<object>.NotFound("Nguoi dung khong ton tai.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Xoa nguoi dung thanh cong.");
        }

        private static UserDto MapUser(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Role = user.Role ?? string.Empty,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }
    }
}


