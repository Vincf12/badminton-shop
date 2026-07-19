using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyAPI.Controllers.Customer
{
    [Route("api/addresses")]
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAddresses()
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(new { message = "Khong the xac dinh nguoi dung hien tai." });
            }

            var addresses = await _addressService.GetAddressesAsync(userId);
            return Ok(addresses);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] AddressUpsertDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(new { message = "Khong the xac dinh nguoi dung hien tai." });
            }

            var result = await _addressService.CreateAddressAsync(userId, dto);
            return ToActionResult(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressUpsertDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(new { message = "Khong the xac dinh nguoi dung hien tai." });
            }

            var result = await _addressService.UpdateAddressAsync(userId, id, dto);
            return ToActionResult(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(new { message = "Khong the xac dinh nguoi dung hien tai." });
            }

            var result = await _addressService.DeleteAddressAsync(userId, id);
            return ToActionResult(result);
        }

        [HttpPut("{id:int}/default")]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(new { message = "Khong the xac dinh nguoi dung hien tai." });
            }

            var result = await _addressService.SetDefaultAddressAsync(userId, id);
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


