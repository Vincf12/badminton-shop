using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace MyAPI.Application.Services.Auth
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;

        public AccountService(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // Lấy thông tin tài khoản hiện tại
        // =========================
        public async Task<ServiceResult<AccountDto>> GetCurrentUserAsync(int userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.UserId == userId)
                .Select(u => MapAccount(u))
                .FirstOrDefaultAsync();

            return user == null
                ? ServiceResult<AccountDto>.NotFound("Tài khoản không tồn tại.")
                : ServiceResult<AccountDto>.Ok(user);
        }

        // =========================
        // Cập nhật thông tin cá nhân
        // =========================
        public async Task<ServiceResult<object>> UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return ServiceResult<object>.NotFound("Tài khoản không tồn tại.");
            }

            // Validate họ tên
            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                return ServiceResult<object>.BadRequest("Họ tên không được để trống.");
            }

            var fullName = dto.FullName.Trim();

            if (fullName.Length < 2 || fullName.Length > 100)
            {
                return ServiceResult<object>.BadRequest(
                    "Họ tên phải từ 2 đến 100 ký tự.");
            }

            // Validate số điện thoại
            string? phone = null;

            if (!string.IsNullOrWhiteSpace(dto.Phone))
            {
                phone = dto.Phone.Trim();

                // 9-11 số, chỉ chứa chữ số
                if (!Regex.IsMatch(phone, @"^\d{9,11}$"))
                {
                    return ServiceResult<object>.BadRequest(
                        "Số điện thoại không hợp lệ.");
                }
            }

            // Validate ngày sinh
            if (dto.Birthdate.HasValue)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);

                if (dto.Birthdate.Value > today)
                {
                    return ServiceResult<object>.BadRequest(
                        "Ngày sinh không hợp lệ.");
                }

                var age = today.Year - dto.Birthdate.Value.Year;

                if (age < 13 || age > 120)
                {
                    return ServiceResult<object>.BadRequest(
                        "Tuổi phải từ 13 đến 120.");
                }
            }

            // Cập nhật
            user.FullName = fullName;
            user.Phone = phone;
            user.Birthdate = dto.Birthdate;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK(
                "Cập nhật thông tin cá nhân thành công.");
        }

        // =========================
        // Đổi mật khẩu
        // =========================
        public async Task<ServiceResult<object>> ChangePasswordAsync(
            int userId,
            ChangePasswordDto dto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return ServiceResult<object>.NotFound(
                    "Tài khoản không tồn tại.");
            }

            // Validate input
            if (string.IsNullOrWhiteSpace(dto.OldPassword))
            {
                return ServiceResult<object>.BadRequest(
                    "Vui lòng nhập mật khẩu hiện tại.");
            }

            if (string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                return ServiceResult<object>.BadRequest(
                    "Vui lòng nhập mật khẩu mới.");
            }

            if (string.IsNullOrWhiteSpace(dto.ConfirmPassword))
            {
                return ServiceResult<object>.BadRequest(
                    "Vui lòng xác nhận mật khẩu mới.");
            }

            // Xác nhận mật khẩu
            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return ServiceResult<object>.BadRequest(
                    "Xác nhận mật khẩu không khớp.");
            }

            // Chính sách mật khẩu
            if (dto.NewPassword.Length < 8)
            {
                return ServiceResult<object>.BadRequest(
                    "Mật khẩu phải có ít nhất 8 ký tự.");
            }

            if (!Regex.IsMatch(dto.NewPassword, @"[A-Z]"))
            {
                return ServiceResult<object>.BadRequest(
                    "Mật khẩu phải chứa ít nhất 1 chữ hoa.");
            }

            if (!Regex.IsMatch(dto.NewPassword, @"[a-z]"))
            {
                return ServiceResult<object>.BadRequest(
                    "Mật khẩu phải chứa ít nhất 1 chữ thường.");
            }

            if (!Regex.IsMatch(dto.NewPassword, @"\d"))
            {
                return ServiceResult<object>.BadRequest(
                    "Mật khẩu phải chứa ít nhất 1 chữ số.");
            }

            // Kiểm tra mật khẩu cũ
            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
            {
                return ServiceResult<object>.BadRequest(
                    "Mật khẩu hiện tại không chính xác.");
            }

            // Không cho phép trùng mật khẩu cũ
            if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.PasswordHash))
            {
                return ServiceResult<object>.BadRequest(
                    "Mật khẩu mới không được trùng với mật khẩu cũ.");
            }

            // Hash mật khẩu mới
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                dto.NewPassword,
                workFactor: 12);

            user.UpdatedAt = DateTime.UtcNow;

            // Nếu dùng JWT có thể tăng phiên bản token
            // user.TokenVersion++;

            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK(
                "Đổi mật khẩu thành công.");
        }

        // =========================
        // Mapping
        // =========================
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

