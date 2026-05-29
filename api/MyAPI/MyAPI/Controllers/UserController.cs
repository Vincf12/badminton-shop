using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        [Authorize] // API này bắt buộc phải có Token mới gọi được
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            return Ok(new { message = "Đây là dữ liệu bí mật từ API sau khi bạn đã login thành công!" });
        }
    }
}
