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
        // Láº¥y thÃ´ng tin tÃ i khoáº£n hiá»‡n táº¡i
        // =========================
        public async Task<ServiceResult<AccountDto>> GetCurrentUserAsync(int userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.UserId == userId)
                .Select(u => MapAccount(u))
                .FirstOrDefaultAsync();

            return user == null
                ? ServiceResult<AccountDto>.NotFound("TÃ i khoáº£n khÃ´ng tá»“n táº¡i.")
                : ServiceResult<AccountDto>.Ok(user);
        }

        // =========================
        // Cáº­p nháº­t há»“ sÆ¡ cÃ¡ nhÃ¢n
        // =========================
        public async Task<ServiceResult<object>> UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return ServiceResult<object>.NotFound("TÃ i khoáº£n khÃ´ng tá»“n táº¡i.");
            }

            // Validate há» tÃªn
            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                return ServiceResult<object>.BadRequest("Há» tÃªn khÃ´ng Ä‘Æ°á»£c Ä‘á»ƒ trá»‘ng.");
            }

            var fullName = dto.FullName.Trim();

            if (fullName.Length < 2 || fullName.Length > 100)
            {
                return ServiceResult<object>.BadRequest(
                    "Há» tÃªn pháº£i tá»« 2 Ä‘áº¿n 100 kÃ½ tá»±.");
            }

            // Validate sá»‘ Ä‘iá»‡n thoáº¡i
            string? phone = null;

            if (!string.IsNullOrWhiteSpace(dto.Phone))
            {
                phone = dto.Phone.Trim();

                // 9-11 sá»‘
                if (!Regex.IsMatch(phone, @"^\d{9,11}$"))
                {
                    return ServiceResult<object>.BadRequest(
                        "Sá»‘ Ä‘iá»‡n thoáº¡i khÃ´ng há»£p lá»‡.");
                }
            }

            // Validate ngÃ y sinh
            if (dto.Birthdate.HasValue)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);

                if (dto.Birthdate.Value > today)
                {
                    return ServiceResult<object>.BadRequest(
                        "NgÃ y sinh khÃ´ng há»£p lá»‡.");
                }

                var age = today.Year - dto.Birthdate.Value.Year;

                if (age < 13 || age > 120)
                {
                    return ServiceResult<object>.BadRequest(
                        "Tuá»•i pháº£i tá»« 13 Ä‘áº¿n 120.");
                }
            }

            // Cáº­p nháº­t
            user.FullName = fullName;
            user.Phone = phone;
            user.Birthdate = dto.Birthdate;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK(
                "Cáº­p nháº­t thÃ´ng tin cÃ¡ nhÃ¢n thÃ nh cÃ´ng.");
        }

        // =========================
        // Äá»•i máº­t kháº©u
        // =========================
        public async Task<ServiceResult<object>> ChangePasswordAsync(
            int userId,
            ChangePasswordDto dto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return ServiceResult<object>.NotFound(
                    "TÃ i khoáº£n khÃ´ng tá»“n táº¡i.");
            }

            // Validate input
            if (string.IsNullOrWhiteSpace(dto.OldPassword))
            {
                return ServiceResult<object>.BadRequest(
                    "Vui lÃ²ng nháº­p máº­t kháº©u hiá»‡n táº¡i.");
            }

            if (string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                return ServiceResult<object>.BadRequest(
                    "Vui lÃ²ng nháº­p máº­t kháº©u má»›i.");
            }

            if (string.IsNullOrWhiteSpace(dto.ConfirmPassword))
            {
                return ServiceResult<object>.BadRequest(
                    "Vui lÃ²ng xÃ¡c nháº­n máº­t kháº©u má»›i.");
            }

            // XÃ¡c nháº­n máº­t kháº©u
            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return ServiceResult<object>.BadRequest(
                    "XÃ¡c nháº­n máº­t kháº©u khÃ´ng khá»›p.");
            }

            // ChÃ­nh sÃ¡ch máº­t kháº©u
            if (dto.NewPassword.Length < 8)
            {
                return ServiceResult<object>.BadRequest(
                    "Máº­t kháº©u pháº£i cÃ³ Ã­t nháº¥t 8 kÃ½ tá»±.");
            }

            if (!Regex.IsMatch(dto.NewPassword, @"[A-Z]"))
            {
                return ServiceResult<object>.BadRequest(
                    "Máº­t kháº©u pháº£i chá»©a Ã­t nháº¥t 1 chá»¯ hoa.");
            }

            if (!Regex.IsMatch(dto.NewPassword, @"[a-z]"))
            {
                return ServiceResult<object>.BadRequest(
                    "Máº­t kháº©u pháº£i chá»©a Ã­t nháº¥t 1 chá»¯ thÆ°á»ng.");
            }

            if (!Regex.IsMatch(dto.NewPassword, @"\d"))
            {
                return ServiceResult<object>.BadRequest(
                    "Máº­t kháº©u pháº£i chá»©a Ã­t nháº¥t 1 chá»¯ sá»‘.");
            }

            // Kiá»ƒm tra máº­t kháº©u cÅ©
            if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
            {
                return ServiceResult<object>.BadRequest(
                    "Máº­t kháº©u hiá»‡n táº¡i khÃ´ng chÃ­nh xÃ¡c.");
            }

            // KhÃ´ng cho phÃ©p trÃ¹ng máº­t kháº©u cÅ©
            if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.PasswordHash))
            {
                return ServiceResult<object>.BadRequest(
                    "Máº­t kháº©u má»›i khÃ´ng Ä‘Æ°á»£c trÃ¹ng vá»›i máº­t kháº©u cÅ©.");
            }

            // Hash máº­t kháº©u má»›i
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                dto.NewPassword,
                workFactor: 12);

            user.UpdatedAt = DateTime.UtcNow;

            // Náº¿u dÃ¹ng JWT cÃ³ thá»ƒ tÄƒng phiÃªn báº£n token
            // user.TokenVersion++;

            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK(
                "Äá»•i máº­t kháº©u thÃ nh cÃ´ng.");
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

