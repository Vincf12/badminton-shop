using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Application.DTOs;
using MyAPI.Services;
using MyAPI.Services.Interfaces;

namespace MyAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(new { message = "Khong the xac dinh nguoi dung hien tai." });
            }

            var result = await _accountService.GetCurrentUserAsync(userId);
            return ToActionResult(result);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(new { message = "Khong the xac dinh nguoi dung hien tai." });
            }

            var result = await _accountService.UpdateProfileAsync(userId, dto);
            return ToActionResult(result);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(new { message = "Khong the xac dinh nguoi dung hien tai." });
            }

            var result = await _accountService.ChangePasswordAsync(userId, dto);
            return ToActionResult(result);
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out userId);
        }

        private IActionResult ToActionResult<T>(ServiceResult<T> result)
        {
            if (result.Succeeded)
            {
                return result.Data != null ? Ok(result.Data) : Ok(new { message = result.Message });
            }

            return StatusCode(result.StatusCode, new { message = result.Message });
        }
    }
}
