using Microsoft.AspNetCore.Mvc;
using WebAppTrafficSign.DTOs;
using WebAppTrafficSign.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace WebAppTrafficSign.Controller
{
    /// API controller quản lý CoinWallet theo requirement
    /// - Users receive 20 initial TSL Coins when registering
    /// - Users manage profiles (personal info, settings, coin balance)
    /// - Admins manage user accounts, permissions, and coin adjustments via the web panel
    [Route("api/wallets")]
    [ApiController]
    [Authorize]
    public class CoinWalletController : ControllerBase
    {
        private readonly ICoinWalletService _coinWalletService;

        public CoinWalletController(ICoinWalletService coinWalletService)
        {
            _coinWalletService = coinWalletService;
        }

        /// Lấy wallet theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            try
            {
                var wallet = await _coinWalletService.GetByIdAsync(id);
                return Ok(wallet);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Lấy wallet theo UserId
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId([FromRoute] int userId)
        {
            try
            {
                var wallet = await _coinWalletService.GetByUserIdAsync(userId);
                return Ok(wallet);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Lấy số dư của user
        [HttpGet("user/{userId}/balance")]
        public async Task<IActionResult> GetBalance([FromRoute] int userId)
        {
            try
            {
                var balance = await _coinWalletService.GetBalanceAsync(userId);
                return Ok(new { UserId = userId, Balance = balance });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Lấy tổng hợp wallets
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            try
            {
                var summary = await _coinWalletService.GetSummaryAsync();
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Tạo wallet mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CoinWalletCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var wallet = await _coinWalletService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = wallet.Id }, wallet);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Cập nhật wallet (chủ yếu cho admin)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] CoinWalletUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var wallet = await _coinWalletService.UpdateAsync(id, request);
                return Ok(wallet);
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
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Admin điều chỉnh coin (reward/penalty)
        [HttpPost("user/{userId}/adjust")]
        public async Task<IActionResult> Adjust([FromRoute] int userId, [FromBody] CoinWalletAdjustRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var wallet = await _coinWalletService.AdjustAsync(userId, request);
                return Ok(wallet);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Cộng thêm xu vào ví (credit)
        [HttpPost("user/{userId}/credit")]
        public async Task<IActionResult> Credit([FromRoute] int userId, [FromBody] CreditRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _coinWalletService.CreditAsync(userId, request.Amount);
                if (result)
                {
                    var wallet = await _coinWalletService.GetByUserIdAsync(userId);
                    return Ok(wallet);
                }
                return BadRequest(new { message = "Failed to credit" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Trừ xu từ ví (debit)
        [HttpPost("user/{userId}/debit")]
        public async Task<IActionResult> Debit([FromRoute] int userId, [FromBody] DebitRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _coinWalletService.DebitAsync(userId, request.Amount);
                if (result)
                {
                    var wallet = await _coinWalletService.GetByUserIdAsync(userId);
                    return Ok(wallet);
                }
                return BadRequest(new { message = "Failed to debit" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Kiểm tra số dư có đủ không
        [HttpPost("user/{userId}/check-balance")]
        public async Task<IActionResult> CheckBalance([FromRoute] int userId, [FromBody] CheckBalanceRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var hasEnough = await _coinWalletService.HasEnoughBalanceAsync(userId, request.Amount);
                return Ok(new { UserId = userId, HasEnoughBalance = hasEnough });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Xóa wallet
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                var result = await _coinWalletService.DeleteAsync(id);
                if (result)
                    return Ok(new { message = "Wallet deleted successfully" });
                return NotFound(new { message = "Wallet not found" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        /// Filter wallets với các điều kiện
        [HttpPost("filter")]
        public async Task<IActionResult> Filter([FromBody] CoinWalletFilterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var wallets = await _coinWalletService.FilterAsync(request);
                return Ok(wallets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }

    /// Helper DTOs cho các endpoints
    public class CreditRequest
    {
        public decimal Amount { get; set; }
    }

    public class DebitRequest
    {
        public decimal Amount { get; set; }
    }

    public class CheckBalanceRequest
    {
        public decimal Amount { get; set; }
    }
}