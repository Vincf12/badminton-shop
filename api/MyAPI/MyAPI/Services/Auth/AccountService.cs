using Microsoft.EntityFrameworkCore;
using MyAPI.Infrastructure.Persistence;
using MyAPI.Domain.Entities;
using MyAPI.Application.DTOs;
using MyAPI.Services.Interfaces;

namespace MyAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;

        public AccountService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<AccountDto>> GetCurrentUserAsync(int userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.UserId == userId)
                .Select(u => MapAccount(u))
                .FirstOrDefaultAsync();

            return user == null
                ? ServiceResult<AccountDto>.NotFound("Nguoi dung khong ton tai.")
                : ServiceResult<AccountDto>.Ok(user);
        }

        public async Task<ServiceResult<object>> UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return ServiceResult<object>.NotFound("Nguoi dung khong ton tai.");
            }

            user.FullName = dto.FullName.Trim();
            user.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();
            user.Birthdate = dto.Birthdate;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ServiceResult<object>.OK("Cap nhat thong tin ca nhan thanh cong.");
        }

        public async Task<ServiceResult<object>> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return ServiceResult<object>.NotFound("Nguoi dung khong ton tai.");
            }

            var passwordMatched = BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash);
            if (!passwordMatched)
            {
                return ServiceResult<object>.BadRequest("Mat khau hien tai khong chinh xac.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ServiceResult<object>.OK("Doi mat khau thanh cong.");
        }

        private static AccountDto MapAccount(User user)
        {
            return new AccountDto
            {
                Id = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Birthdate = user.Birthdate,
                Role = user.Role
            };
        }
    }
}
