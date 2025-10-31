using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppTrafficSign.Data;
using WebAppTrafficSign.Models;
using WebAppTrafficSign.DTOs;
using WebAppTrafficSign.Services;



namespace WebAppTrafficSign.Controller
{

    /// API Controller cho quản lý người dùng
    /// Sử dụng UserService để xử lý business logic
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// Đăng ký tài khoản mới - User nhận 20 coin ban đầu
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userService.RegisterAsync(request);
                return Ok(new { message = "Đăng ký thành công. Bạn đã nhận 20 coin khởi đầu.", user });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log inner exception để debug
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += $" | Inner Exception: {ex.InnerException.Message}";
                }
                return StatusCode(500, new { message = "Lỗi hệ thống: " + errorMessage, details = ex.StackTrace });
            }
        }

        /// Đăng nhập - Trả về token và thông tin user
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _userService.LoginAsync(request);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Lấy thông tin user theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = "User not found" });
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Lấy thông tin profile đầy đủ của user (bao gồm coin balance, contributions, votes)
        [HttpGet("{id}/profile")]
        public async Task<IActionResult> GetUserProfile([FromRoute] int id)
        {
            try
            {
                var profile = await _userService.GetUserProfileAsync(id);
                return Ok(profile);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Cập nhật thông tin profile (username, email, phone)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile([FromRoute] int id, [FromBody] UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userService.UpdateProfileAsync(id, request);
                return Ok(new { message = "Cập nhật profile thành công.", user });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Đổi mật khẩu
        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> ChangePassword([FromRoute] int id, [FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _userService.ChangePasswordAsync(id, request);
                if (result)
                    return Ok(new { message = "Đổi mật khẩu thành công." });
                return BadRequest(new { message = "Không thể đổi mật khẩu." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Yêu cầu reset password (gửi email)
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _userService.ForgotPasswordAsync(request);
                if (result)
                    return Ok(new { message = "Email reset password đã được gửi. Vui lòng kiểm tra hộp thư." });
                return NotFound(new { message = "Email không tồn tại trong hệ thống." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Lấy số dư coin trong ví
        [HttpGet("{id}/wallet/balance")]
        public async Task<IActionResult> GetWalletBalance([FromRoute] int id)
        {
            try
            {
                var balance = await _userService.GetWalletBalanceAsync(id);
                return Ok(balance);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Nạp coin vào ví - $1 = 10 Coins
        [HttpPost("{id}/wallet/top-up")]
        public async Task<IActionResult> TopUpCoins([FromRoute] int id, [FromBody] CoinTopUpRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.AmountInDollars <= 0)
                return BadRequest(new { message = "Số tiền phải lớn hơn 0" });

            try
            {
                var response = await _userService.TopUpCoinsAsync(id, request);
                return Ok(new { message = "Nạp coin thành công.", data = response });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        /// Xóa tài khoản user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (result)
                    return Ok(new { message = "Xóa user thành công." });
                return NotFound(new { message = "User not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
