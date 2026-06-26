using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyAPI.Infrastructure.Persistence;
using MyAPI.Domain.Entities;
using MyAPI.Application.DTOs;
using MyAPI.Services.Interfaces;

namespace MyAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ServiceResult<object>> RegisterAsync(RegisterDto dto)
        {
            var emailExists = await _context.Users.AnyAsync(x => x.Email == dto.Email);
            if (emailExists)
            {
                return ServiceResult<object>.BadRequest("Email da ton tai tren he thong.");
            }

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                Phone = dto.Phone
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Dang ky thanh cong.");
        }

        public async Task<ServiceResult<object>> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (user == null)
            {
                return ServiceResult<object>.Error(StatusCodes.Status401Unauthorized, "Email hoac mat khau khong chinh xac.");
            }

            var passwordMatched = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!passwordMatched)
            {
                return ServiceResult<object>.Error(StatusCodes.Status401Unauthorized, "Email hoac mat khau khong chinh xac.");
            }

            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                return ServiceResult<object>.Error(StatusCodes.Status500InternalServerError, "Cau hinh he thong loi: thieu JWT Key.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "customer")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return ServiceResult<object>.Ok(new
            {
                message = "Dang nhap thanh cong.",
                token = jwt,
                user = new
                {
                    user.Email,
                    user.FullName,
                    user.Role
                }
            });
        }
    }
}
