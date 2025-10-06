using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebAppTrafficSign.DTOs;
using WebAppTrafficSign.Services;

namespace WebAppTrafficSign.Controllers
{
    public class AuthControllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class UsersController : ControllerBase
        {
            private readonly IUserService _userService;

            public UsersController(IUserService userService)
            {
                _userService = userService;
            }

            // Đăng ký tài khoản
            [HttpPost("register")]
            [AllowAnonymous]
            public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                try
                {
                    var user = await _userService.RegisterAsync(request);
                    // 201 Created với location header trỏ tới user mới
                    return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
                }
                catch (System.InvalidOperationException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }
            // Đăng nhập
            [HttpPost("login")]
            [AllowAnonymous]
            public async Task<IActionResult> Login([FromBody] LoginRequest request)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                try
                {
                    var authResponse = await _userService.LoginAsync(request);
                    return Ok(authResponse);
                }
                catch (System.InvalidOperationException ex)
                {
                    return Unauthorized(new { message = ex.Message });
                }
            }

            // Lấy thông tin người dùng hiện tại (dùng token)
            [HttpGet("me")]
            [Authorize]
            public async Task<IActionResult> GetCurrentUser()
            {
                // Giả sử bạn lưu Id người dùng vào claim "id" khi tạo token
                var idString = User.FindFirst("id")?.Value;
                if (!int.TryParse(idString, out var id))
                    return Unauthorized();

                var user = await _userService.GetByIdAsync(id);
                if (user == null) return NotFound();
                return Ok(user);
            }

            // Lấy thông tin người dùng theo Id
            [HttpGet("{id:int}")]
            [Authorize(Roles = "Admin,Staff")]
            public async Task<IActionResult> GetById(int id)
            {
                var user = await _userService.GetByIdAsync(id);
                return user == null ? NotFound() : Ok(user);
            }

            // Cập nhật hồ sơ người dùng
            [HttpPut("{id:int}")]
            [Authorize]
            public async Task<IActionResult> UpdateProfile(int id, [FromBody] UserUpdateRequest request)
            {
                // Kiểm tra người dùng hiện tại có quyền chỉnh sửa bản thân hay admin mới được chỉnh sửa người khác
                var currentUserId = int.Parse(User.FindFirst("id").Value);
                var currentRole = User.FindFirst("role").Value; // lấy role gắn vào token
                if (currentUserId != id && currentRole != "Admin")
                    return Forbid();

                var user = await _userService.UpdateProfileAsync(id, request);
                return Ok(user);
            }

            // Đổi mật khẩu
            [HttpPut("{id:int}/change-password")]
            [Authorize]
            public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
            {
                var currentUserId = int.Parse(User.FindFirst("id").Value);
                if (currentUserId != id) return Forbid();

                try
                {
                    await _userService.ChangePasswordAsync(id, request);
                    return NoContent();
                }
                catch (System.InvalidOperationException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            // Quên mật khẩu (gửi email reset)
            [HttpPost("forgot-password")]
            [AllowAnonymous]
            public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
            {
                var result = await _userService.ForgotPasswordAsync(request);
                // Không tiết lộ email có tồn tại hay không
                return Ok(new { success = result });
            }

            // Xóa người dùng (Admin)
            [HttpDelete("{id:int}")]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> Delete(int id)
            {
                var result = await _userService.DeleteUserAsync(id);
                return result ? NoContent() : NotFound();
            }
        }
    }
}
